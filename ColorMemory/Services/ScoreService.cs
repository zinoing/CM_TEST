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

        public async Task<double?> GetWeeklyScoreAsync(string playerId)
        {
            return await _weeklyRankingDb.GetScoreAsyncById(playerId);
        }

        public async Task<double?> GetNationalScoreAsync(string playerId)
        {
            return await _nationalRankingDb.GetScoreAsyncById(playerId);
        }

        public async Task<List<ScoreDTO>> GetTopWeeklyScoresAsync(int count)
        {
            return await _weeklyRankingDb.GetTopRanksAsync(count);
        }

        public async Task<List<ScoreDTO>> GetTopNationalScoresAsync(int count)
        {
            return await _nationalRankingDb.GetTopRanksAsync(count);
        }

        public async Task<List<ScoreDTO>> GetSurroundingWeeklyScoresAsync(string playerId, int range)
        {
            return await _weeklyRankingDb.GetSurroundingWeeklyScoresAsync(playerId, range);
        }
    }
}
