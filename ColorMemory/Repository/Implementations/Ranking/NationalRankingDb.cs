using Microsoft.Extensions.Configuration;

namespace ColorMemory.Repository.Implementations
{
    public class NationalRankingDb : BaseRankingDb
    {
        public NationalRankingDb(ILogger<NationalRankingDb> logger, IConfiguration configuration)
            : base(logger, configuration, "national_rankings") { }
    }
}
