using ColorMemory.DTO;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ColorMemory.Repository.Implementations
{
    public abstract class BaseRankingDb : BaseRedisDb<BaseRankingDb>
    {
        protected readonly string _key;

        protected BaseRankingDb(ILogger<BaseRankingDb> logger, IConfiguration configuration, string key) 
            : base(logger, configuration)
        {   
            _key = key;
        }

        public async Task SaveScoreAsync(ScoreDTO scoreInfo)
        {
            var id = scoreInfo.PlayerId.ToString();
            var score = scoreInfo.Score;
            await _database.SortedSetAddAsync(_key, id, score);
        }

        public async Task<double?> GetScoreAsyncById(string userId)
        {
            var score = await _database.SortedSetScoreAsync(_key, userId);
            return score.HasValue ? score.Value : null;
        }

        public async Task<long?> GetRankAsyncById(string userId)
        {
            var rank = await _database.SortedSetRankAsync(_key, userId, Order.Descending);
            return rank.HasValue ? rank.Value + 1 : null;
        }

        public async Task<List<ScoreDTO>> GetTopRanksAsync(int count)
        {
            var topScores = await _database.SortedSetRangeByRankWithScoresAsync(_key, 0, count - 1, Order.Descending);

            List<ScoreDTO> scores = new List<ScoreDTO>();

            foreach (var score in topScores)
            {
                scores.Add(new ScoreDTO(score.Element, (int)score.Score));
            }

            return scores;
        }

        public async Task<List<ScoreDTO>> GetSurroundingWeeklyScoresAsync(string playerId, int range)
        {
            var rank = await _database.SortedSetRankAsync(_key, playerId, Order.Descending);

            if (rank == null)
                return new List<ScoreDTO>();

            long start = Math.Max(0, rank.Value - range);
            long end = rank.Value + range;

            var surroundingScores = await _database.SortedSetRangeByRankWithScoresAsync(_key, start, end, Order.Descending);

            List<ScoreDTO> scores = new List<ScoreDTO>();

            foreach (var score in surroundingScores)
            {
                scores.Add(new ScoreDTO(score.Element, (int)score.Score));
            }

            return scores;
        }

        public async Task<bool> DeleteScoreAsyncById(string userId)
        {
            return await _database.SortedSetRemoveAsync(_key, userId);
        }
    }

}
