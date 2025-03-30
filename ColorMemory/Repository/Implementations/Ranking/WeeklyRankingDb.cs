using ColorMemory.Controllers;
using ColorMemory.DTO;
using StackExchange.Redis;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace ColorMemory.Repository.Implementations
{
    public class WeeklyRankingDb : BaseRankingDb
    {
        public WeeklyRankingDb(ILogger<WeeklyRankingDb> logger, IConfiguration configuration)
            : base(logger, configuration, "weekly_rankings") { }
    }
}
