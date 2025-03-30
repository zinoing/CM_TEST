using ColorMemory.Data;
using ColorMemory.DTO;
using ColorMemory.Repository.Implementations;
using Microsoft.EntityFrameworkCore;

namespace ColorMemory.Services
{
    public class PlayerService
    {
        private readonly PlayerDb _playerDb;
        public PlayerService(PlayerDb playerDb)
        {
            _playerDb = playerDb;
        }

        public async Task<Player> AddPlayerAsync(PlayerDTO playerInfo) {
            var player = await _playerDb.AddPlayerAsync(playerInfo);

            return player;
        }

        public async Task<Player> GetPlayerInfo(string playerId)
        {
            var player = await _playerDb.GetPlayerAsync(playerId);

            return player;
        }

        public async Task<string> GetNameAsync(string playerId)
        {
            var name = await _playerDb.GetNameAsync(playerId);

            return name;
        }

        public async Task<int> GetScoreAsync(string playerId)
        {
            var score = await _playerDb.GetScoreAsync(playerId);

            return score;
        }

        public async Task<int> GetMoneyAsync(string playerId)
        {
            var money = await _playerDb.GetMoneyAsync(playerId);

            return money;
        }
    }
}
