using StackExchange.Redis;

namespace ColorMemory.Repository.Implementations
{
    public abstract class BaseRedisDb<T>
    {
        protected readonly ILogger<T> _logger;
        protected readonly IDatabase _database;

        protected BaseRedisDb(ILogger<T> logger, IConfiguration configuration)
        {
            _logger = logger;
            var redisHost = configuration["Redis:Host"];
            var redisPort = configuration["Redis:Port"];
            var connectionString = $"{redisHost}:{redisPort}";
            _database = ConnectionMultiplexer.Connect(connectionString).GetDatabase();
        }
    }
}
