using ColorMemory.Data;
using StackExchange.Redis;

namespace ColorMemory.Repository.Implementations
{
    /// <summary>
    /// This class does not contains PlayerArtworks
    /// </summary>
    public abstract class BasePlayerDb : BaseRedisDb<BasePlayerDb>
    {
        protected readonly string _key;

        protected BasePlayerDb(ILogger<BasePlayerDb> logger, IConfiguration configuration, string key)
            : base(logger, configuration)
        {
            _key = key;
        }

        protected HashEntry[] PlayerToHash(Player player)
        {
            return
            [
                new HashEntry("PlayerId", player.PlayerId),
                new HashEntry("Name", player.Name),
                new HashEntry("IconId", player.IconId),
                new HashEntry("Score", player.Score),
                new HashEntry("Money", player.Money)
            ];
        }

        protected Player PlayerFromHash(HashEntry[] hash)
        {
            var dict = hash.ToDictionary(
                h => h.Name.ToString(),
                h => h.Value.ToString()
            );

            return new Player
            {
                PlayerId = dict.GetValueOrDefault("PlayerId"),
                Name = dict.GetValueOrDefault("Name"),
                IconId = int.Parse(dict.GetValueOrDefault("IconId", "0")),
                Score = int.Parse(dict.GetValueOrDefault("Score", "0")),
                Money = int.Parse(dict.GetValueOrDefault("Money", "0"))
            };
        }
    }
}
