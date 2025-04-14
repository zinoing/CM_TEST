using ColorMemory.Data;
using ColorMemory.DTO;
using ColorMemory.Repository.Implementations;
using Microsoft.EntityFrameworkCore;

namespace ColorMemory.Services
{
    public class ScoreService
    {
        private readonly WeeklyRankingDb _weeklyRankingDb;
        private readonly PlayerDb _playerDb;

        public ScoreService(WeeklyRankingDb weeklyRankingDb, PlayerDb playerDb)
        {
            _weeklyRankingDb = weeklyRankingDb;
            _playerDb = playerDb;
        }

        public async Task<bool> UpdateWeeklyScoreAsync(ScoreDTO scoreInfo)
        {

            var result = await _playerDb.SetScoreAsync(scoreInfo.PlayerId, scoreInfo.Score);
            if (!result) return result;
            result = await _weeklyRankingDb.SaveScoreAsync(scoreInfo);
            return result;

        }

        public async Task<int> GetHighScoreAsync(string playerId)
        {
            Player player = await _playerDb.GetPlayerAsync(playerId);
            return player.Score;
        }

        public async Task<int> GetWeeklyScoreAsIntAsync(string playerId)
        {
            return await _weeklyRankingDb.GetHighScoreAsIntAsyncById(playerId);
        }

        public async Task<PlayerRankingDTO> GetWeeklyScoreAsDTOAsync(string playerId)
        {
            Player player = await _playerDb.GetPlayerAsync(playerId);
            int score = await _weeklyRankingDb.GetHighScoreAsIntAsyncById(playerId);
            int ranking = await _weeklyRankingDb.GetRankingAsIntAsyncById(playerId);
            if (player != null)
            {
                return new PlayerRankingDTO(playerId, score, player.Name, player.IconId, ranking);
            }
            else
            {
                return null;
            }
        }

        public async Task<List<PlayerRankingDTO>> GetTopWeeklyScoresAsync(int count)
        {
            return await _weeklyRankingDb.GetTopRanksAsync(count);
        }

        public async Task<List<PlayerRankingDTO>> GetSurroundingWeeklyScoresByIdAsync(string playerId, int range)
        {
            return await _weeklyRankingDb.GetSurroundingWeeklyScoresByIdAsync(playerId, range);
        }

        public async Task<List<List<PlayerRankingDTO>>> GetSurroundingWeeklyScoresByScoreAsync(int score, int range)
        {
            return await _weeklyRankingDb.GetSurroundingWeeklyScoresByScoreAsync(score, range);
        }

        public async Task<int> GetPlayerWeeklyRankingAsync(string playerId)
        {
            int ranking = await _weeklyRankingDb.GetRankingAsIntAsyncById(playerId);
            return ranking;
        }
    }
}
