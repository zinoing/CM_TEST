using ColorMemory.Services;
using Microsoft.Extensions.Configuration;

namespace ColorMemory.Repository.Implementations
{
    public class NationalRankingDb : BaseRankingDb
    {
        public NationalRankingDb(PlayerService playerService, ILogger<NationalRankingDb> logger, IConfiguration configuration)
            : base(playerService, logger, configuration, "national_rankings") { }
    }
}
