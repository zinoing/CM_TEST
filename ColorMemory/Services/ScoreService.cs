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

            var result = await _playerDb.SetScoreAsync(scoreInfo.PlayerId, scoreInfo.Score);
            if (!result) return result;
            result = await _weeklyRankingDb.SaveScoreAsync(scoreInfo);
            return result;

        }

        public async Task<bool> UpdateNationalScoreAsync(ScoreDTO scoreInfo)
        {
            var result = await _nationalRankingDb.SaveScoreAsync(scoreInfo);
            if (!result) return result;
            result = await _playerDb.SetScoreAsync(scoreInfo.PlayerId, scoreInfo.Score);
            return result;
        }

        public async Task<int?> GetWeeklyScoreAsIntAsync(string playerId)
        {
            return await _weeklyRankingDb.GetHighScoreAsIntAsyncById(playerId);
        }

        public async Task<PlayerRankingDTO> GetWeeklyScoreAsDTOAsync(string playerId)
        {
            Player player = await _playerDb.GetPlayerAsync(playerId);
            int ranking = await _weeklyRankingDb.GetRankingAsIntAsyncById(playerId);
            if (player != null)
            {
                return new PlayerRankingDTO(playerId, player.Score, player.Name, player.IconId, ranking);
            }
            else
            {
                return null;
            }
        }

        public async Task<int?> GetNationalScoreAsync(string playerId)
        {
            return await _nationalRankingDb.GetHighScoreAsIntAsyncById(playerId);
        }

        public async Task<List<PlayerRankingDTO>> GetTopWeeklyScoresAsync(int count)
        {
            return await _weeklyRankingDb.GetTopRanksAsync(count);
        }

        public async Task<List<PlayerRankingDTO>> GetTopNationalScoresAsync(int count)
        {
            return await _nationalRankingDb.GetTopRanksAsync(count);
        }

        public async Task<List<PlayerRankingDTO>> GetSurroundingWeeklyScoresAsync(string playerId, int range)
        {
            return await _weeklyRankingDb.GetSurroundingWeeklyScoresAsync(playerId, range);
        }

        public async Task<int> GetPlayerWeeklyRankingAsync(string playerId)
        {
            int ranking = await _weeklyRankingDb.GetRankingAsIntAsyncById(playerId);
            return ranking;
        }
    }
}
