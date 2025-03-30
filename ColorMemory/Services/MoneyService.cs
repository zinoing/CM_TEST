using ColorMemory.Data;
using ColorMemory.DTO;
using ColorMemory.Repository.Implementations;
using Microsoft.EntityFrameworkCore;

namespace ColorMemory.Services
{
    public class MoneyService
    {
        private readonly PlayerDb _playerDb;
        public MoneyService(PlayerDb playerDb)
        {
            _playerDb = playerDb;
        }

        public async Task<int> GetPlayerMoneyAsync(string playerId)
        {
            int money = await _playerDb.GetMoneyAsync(playerId);

            return money;
        }

        public async Task<bool> UpdatePlayerMoneyAsync(MoneyDTO moneyInfo)
        {
            bool result = await _playerDb.SetMoneyAsync(moneyInfo.PlayerId, moneyInfo.Money);

            return result;
        }

        public async Task<bool> EarnPlayerMoneyAsync(string playerId, int moneyToEarn)
        {
            int currentMoney = await GetPlayerMoneyAsync(playerId);

            bool result = await _playerDb.SetMoneyAsync(playerId, currentMoney + moneyToEarn);

            return result;
        }

        public async Task<bool> PayPlayerMoneyAsync(string playerId, int moneyToPay)
        {
            int currentMoney = await GetPlayerMoneyAsync(playerId);

            if (moneyToPay > currentMoney) { 
                return false;
            }

            MoneyDTO moneyInfo = new MoneyDTO(playerId, currentMoney - moneyToPay);
            bool result = await UpdatePlayerMoneyAsync(moneyInfo);
            return result;
        }
    }
}
