using ColorMemory.Data;
using ColorMemory.DTO;
using ColorMemory.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Numerics;

namespace ColorMemory.Repository.Implementations
{
    public class HintDb
    {
        private readonly GameDbContext _context;

        public HintDb(ILogger<PlayerDb> logger, GameDbContext context, IConfiguration configuration)
        {
            _context = context;
        }

        public async Task<int> GetHintPriceAsync(HintType hintType)
        {
            Hint hintInfo = await _context.Hints.FirstOrDefaultAsync(h => h.HintType == hintType);

            if (hintInfo == null) return -1;
            return hintInfo.Price;
        }
    }
}
