using ColorMemory.Services;
using Microsoft.Extensions.Configuration;

namespace ColorMemory.Repository.Implementations
{
    public class NationalRankingDb : BaseRankingDb
    {
        public NationalRankingDb(PlayerDb playerDb, ILogger<NationalRankingDb> logger, IConfiguration configuration)
            : base(playerDb, logger, configuration, "national_rankings") { }
    }
}
