using ColorMemory.Controllers;
using ColorMemory.DTO;
using StackExchange.Redis;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using ColorMemory.Services;

namespace ColorMemory.Repository.Implementations
{
    public class WeeklyRankingDb : BaseRankingDb
    {
        public WeeklyRankingDb(PlayerService playerService, ILogger<WeeklyRankingDb> logger, IConfiguration configuration)
            : base(playerService, logger, configuration, "weekly_rankings") { }
    }
}
