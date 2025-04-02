using ColorMemory.Data;
using ColorMemory.DTO;
using ColorMemory.Repository.Implementations;
using Microsoft.EntityFrameworkCore;

namespace ColorMemory.Services
{
    public class ScoreService
    {
        private readonly WeeklyRankingDb _weeklyRankingDb;
        private readonly NationalRankingDb _nationalRankingDb;
        private readonly PlayerDb _playerDb;

        public ScoreService(WeeklyRankingDb weeklyRankingDb, NationalRankingDb nationalRankingDb, PlayerDb playerDb)
        {
            _weeklyRankingDb = weeklyRankingDb;
            _nationalRankingDb = nationalRankingDb;
            _playerDb = playerDb;
        }

        public async Task<bool> UpdateWeeklyScoreAsync(ScoreDTO scoreInfo)
        {
            await _weeklyRankingDb.SaveScoreAsync(scoreInfo);
            var result = await _playerDb.SetScoreAsync(scoreInfo.PlayerId, scoreInfo.Score);
            return result;
        }

        public async Task UpdateNationalScoreAsync(ScoreDTO scoreInfo)
        {
            await _nationalRankingDb.SaveScoreAsync(scoreInfo);
        }

        public async Task<int?> GetWeeklyScoreAsIntAsync(string playerId)
        {
            return await _weeklyRankingDb.GetScoreAsIntAsyncById(playerId);
        }

        public async Task<PlayerScoreDTO> GetWeeklyScoreAsDTOAsync(string playerId)
        {
            return await _playerDb.GetPlayerScoreDTOAsync(playerId);
        }

        public async Task<int?> GetNationalScoreAsync(string playerId)
        {
            return await _nationalRankingDb.GetScoreAsIntAsyncById(playerId);
        }

        public async Task<List<PlayerScoreDTO>> GetTopWeeklyScoresAsync(int count)
        {
            return await _weeklyRankingDb.GetTopRanksAsync(count);
        }

        public async Task<List<PlayerScoreDTO>> GetTopNationalScoresAsync(int count)
        {
            return await _nationalRankingDb.GetTopRanksAsync(count);
        }

        public async Task<List<PlayerScoreDTO>> GetSurroundingWeeklyScoresAsync(string playerId, int range)
        {
            return await _weeklyRankingDb.GetSurroundingWeeklyScoresAsync(playerId, range);
        }

        public async Task<int> GetPlayerWeeklyRankingAsync(string playerId)
        {
            long? rank = await _weeklyRankingDb.GetRankAsyncById(playerId);

            if (rank == null)
                return -1;

            return (int)rank.Value;
        }
    }
}
