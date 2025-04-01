using ColorMemory.DTO;
using ColorMemory.Services;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ColorMemory.Repository.Implementations
{
    public abstract class BaseRankingDb : BaseRedisDb<BaseRankingDb>
    {
        readonly PlayerService _playerService;
        protected readonly string _key;

        protected BaseRankingDb(PlayerService playerService, ILogger<BaseRankingDb> logger, IConfiguration configuration, string key) 
            : base(logger, configuration)
        {
            _playerService = playerService;
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

        public async Task<List<PlayerScoreDTO>> GetTopRanksAsync(int count)
        {
            var topScores = await _database.SortedSetRangeByRankWithScoresAsync(_key, 0, count - 1, Order.Descending);

            List<PlayerScoreDTO> playerScores = new List<PlayerScoreDTO>();

            foreach (var score in topScores)
            {
                string name = await _playerService.GetNameAsync(score.Element.ToString());
                string IconId = await _playerService.GetIconIdAsync(score.Element.ToString());
                playerScores.Add(new PlayerScoreDTO(score.Element, name, IconId, (int)score.Score));
            }

            return playerScores;
        }

        public async Task<List<PlayerScoreDTO>> GetSurroundingWeeklyScoresAsync(string playerId, int range)
        {
            var rank = await _database.SortedSetRankAsync(_key, playerId, Order.Descending);

            if (rank == null)
                return new List<PlayerScoreDTO>();

            long start = Math.Max(0, rank.Value - range);
            long end = rank.Value + range;

            var surroundingScores = await _database.SortedSetRangeByRankWithScoresAsync(_key, start, end, Order.Descending);

            List<PlayerScoreDTO> playerScores = new List<PlayerScoreDTO>();

            foreach (var score in surroundingScores)
            {
                string name = await _playerService.GetNameAsync(score.Element.ToString());
                string IconId = await _playerService.GetIconIdAsync(score.Element.ToString());
                playerScores.Add(new PlayerScoreDTO(score.Element, name, IconId, (int)score.Score));
            }

            return playerScores;
        }

        public async Task<bool> DeleteScoreAsyncById(string userId)
        {
            return await _database.SortedSetRemoveAsync(_key, userId);
        }
    }

}
