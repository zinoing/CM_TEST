using ColorMemory.Data;
using ColorMemory.DTO;
using ColorMemory.Repository.Implementations;
using Microsoft.EntityFrameworkCore;

namespace ColorMemory.Services
{
    public class HintService
    {
        private readonly PlayerDb _playerDb;
        private readonly MoneyService _moneyService;
        public HintService(PlayerDb playerDb, MoneyService moneyService)
        {
            _playerDb = playerDb;
            _moneyService = moneyService;
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
