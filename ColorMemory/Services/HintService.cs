using ColorMemory.Data;
using ColorMemory.DTO;
using ColorMemory.Repository.Implementations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;

namespace ColorMemory.Services
{
    public class HintService
    {
        private readonly PlayerDb _playerDb;
        private readonly HintDb _hintDb;
        private readonly MoneyService _moneyService;
        public HintService(PlayerDb playerDb, HintDb hintDb, MoneyService moneyService)
        {
            _playerDb = playerDb;
            _hintDb = hintDb;
            _moneyService = moneyService;
        }

        public async Task<int> GetHintPriceAsync(HintType type)
        {
            return await _hintDb.GetHintPriceAsync(type);
        }

        public async Task<bool> BuyPlayerHintAsync(HintDTO hintInfo)
        {
            bool result = false;
            if (hintInfo.Type == HintType.OneZoneHint)
            {
                // 검증 단계 필요
                // 재화 사용 단계 필요
                result = await _moneyService.PayPlayerMoneyAsync(hintInfo.PlayerId, 50);
            }
            else
            {
                // 검증 단계 필요
                // 재화 사용 단계 필요
                result = await _moneyService.PayPlayerMoneyAsync(hintInfo.PlayerId, 100);
            }

            return result;
        }
    }
}
