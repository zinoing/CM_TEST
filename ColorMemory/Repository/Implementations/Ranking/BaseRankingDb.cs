using ColorMemory.Data;
using ColorMemory.DTO;
using ColorMemory.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ColorMemory.Repository.Implementations
{
    public abstract class BaseRankingDb : BaseRedisDb<BaseRankingDb>
    {
        private readonly PlayerDb _playerDb;
        protected readonly string _key;

        protected BaseRankingDb(PlayerDb playerDb, ILogger<BaseRankingDb> logger, IConfiguration configuration, string key) 
            : base(logger, configuration)
        {
            _playerDb = playerDb;
            _key = key;
        }

        public async Task<bool> SaveScoreAsync(ScoreDTO scoreInfo)
        {
            var playerId = scoreInfo.PlayerId;
            var newScore = scoreInfo.Score;

            var previousScore = await _playerDb.GetScoreAsync(playerId);
            if (previousScore == -1) return false;

            if (previousScore <= newScore)
            {
                await _database.SortedSetAddAsync(_key, playerId, newScore);
            }
            return true;
        }

        public async Task<int?> GetHighScoreAsIntAsyncById(string userId)
        {
            var score = await _database.SortedSetScoreAsync(_key, userId);
            if (score.HasValue)
            {
                return (int?)score.Value;
            }
            return null;
        }

        public async Task<int> GetRankingAsIntAsyncById(string userId)
        {
            var rank = await _database.SortedSetRankAsync(_key, userId, Order.Descending);
            return rank.HasValue ? (int)rank.Value + 1 : -1;
        }

        public async Task<List<PlayerRankingDTO>> GetTopRanksAsync(int count)
        {
            var topScores = await _database.SortedSetRangeByRankWithScoresAsync(_key, 0, count - 1, Order.Descending);

            List<PlayerRankingDTO> playerScores = new List<PlayerRankingDTO>();

            foreach (var score in topScores)
            {
                string name = await _playerDb.GetNameAsync(score.Element.ToString());
                int iconId = await _playerDb.GetIconIdAsync(score.Element.ToString());
                int ranking = await GetRankingAsIntAsyncById(score.Element.ToString());

                playerScores.Add(new PlayerRankingDTO(score.Element, (int)score.Score, name, iconId, ranking));
            }

            return playerScores;
        }

        public async Task<List<PlayerRankingDTO>> GetSurroundingWeeklyScoresAsync(string playerId, int range)
        {
            var rank = await _database.SortedSetRankAsync(_key, playerId, Order.Descending);

            if (rank == null)
                return new List<PlayerRankingDTO>();

            long start = Math.Max(0, rank.Value - range);
            long end = rank.Value + range;

            var surroundingScores = await _database.SortedSetRangeByRankWithScoresAsync(_key, start, end, Order.Descending);

            List<PlayerRankingDTO> playerScores = new List<PlayerRankingDTO>();

            foreach (var score in surroundingScores)
            {
                string name = await _playerDb.GetNameAsync(score.Element.ToString());
                int iconId = await _playerDb.GetIconIdAsync(score.Element.ToString());
                int ranking = await GetRankingAsIntAsyncById(score.Element.ToString());

                playerScores.Add(new PlayerRankingDTO(score.Element, (int)score.Score, name, iconId, ranking));
            }

            return playerScores;
        }

        public async Task<bool> DeleteScoreAsyncById(string userId)
        {
            return await _database.SortedSetRemoveAsync(_key, userId);
        }
    }

}
