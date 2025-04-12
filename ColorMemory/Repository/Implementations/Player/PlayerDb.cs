using ColorMemory.Data;
using ColorMemory.DTO;
using ColorMemory.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Numerics;

namespace ColorMemory.Repository.Implementations
{
    public class PlayerDb : BasePlayerDb
    {
        private readonly GameDbContext _context;

        public PlayerDb(ILogger<PlayerDb> logger, GameDbContext context, IConfiguration configuration)
            : base(logger, configuration, "player_info")
        {
            _context = context;
        }

        private async Task<Player> FindPlayerInRedisDbAsync(string playerId)
        {
            string redisKey = $"player:{playerId}";

            if (await _database.KeyExistsAsync(redisKey))
            {
                var hash = await _database.HashGetAllAsync(redisKey);

                return PlayerFromHash(hash);
            }

            return null;
        }

        private async Task<Player> FindPlayerInRDSAsync(string playerId)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null)
            {
                return null;
            }
            else
            {
                return player;
            }
        }

        public async Task<Player> AddPlayerAsync(PlayerDTO playerInfo)
        {
            var player = await FindPlayerAsync(playerInfo.PlayerId);
            if (player != null)
            {
                return player;
            }

            var newPlayer = new Player
            {
                PlayerId = playerInfo.PlayerId,
                Name = playerInfo.Name,
                IconId = playerInfo.IconId,
                Score = playerInfo.Score,
                Money = playerInfo.Money,
                PlayerArtworks = new List<PlayerArtwork>()
            };

            var artworks = await _context.Artworks.ToListAsync();

            foreach (var artwork in artworks)
            {
                var stages = Enumerable.Range(0, 16).ToDictionary(i => i, i => new StageDTO
                {
                    Status = StageStauts.Lock,
                    Rank = Rank.NONE,
                    HintUsage = -1,
                    IncorrectCnt = -1
                });

                stages[0].Status = StageStauts.Open;

                var newPlayerArtwork = new PlayerArtwork
                {
                    PlayerId = newPlayer.PlayerId,
                    Player = newPlayer,
                    ArtworkId = artwork.ArtworkId,
                    Artwork = artwork,
                    Rank = Rank.NONE,
                    HasIt = false,
                    TotalMistakesAndHints = 0,
                    Stages = JsonConvert.SerializeObject(stages)
                };

                _context.PlayerArtworks.Add(newPlayerArtwork);
                newPlayer.PlayerArtworks.Add(newPlayerArtwork);
            }

            _context.Players.Add(newPlayer);
            await _context.SaveChangesAsync();

            string redisKey = $"player:{playerInfo.PlayerId}";
            await _database.HashSetAsync(redisKey, PlayerToHash(newPlayer));
            await _database.KeyExpireAsync(redisKey, TimeSpan.FromMinutes(30));

            return newPlayer;
        }

        private async Task<Player> FindPlayerAsync(string playerId)
        {
            var playerFromCache = await FindPlayerInRedisDbAsync(playerId);
            if (playerFromCache != null)
                return playerFromCache;

            var playerFromDb = await FindPlayerInRDSAsync(playerId);
            if (playerFromDb != null)
            {
                string redisKey = $"player:{playerId}";
                await _database.HashSetAsync(redisKey, PlayerToHash(playerFromDb));
                await _database.KeyExpireAsync(redisKey, TimeSpan.FromMinutes(30));
            }

            return playerFromDb;
        }

        // get functions
        public async Task<Player> GetPlayerAsync(string playerId)
        {
            Player player = await FindPlayerInRDSAsync(playerId);
            if (player != null)
            {
                return player;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> GetNameAsync(string playerId)
        {
            Player player = await FindPlayerAsync(playerId);
            if (player != null)
            {
                return player.Name;
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task<int> GetIconIdAsync(string playerId)
        {
            Player player = await FindPlayerAsync(playerId);
            if (player != null)
            {
                return player.IconId;
            }
            else
            {
                return -1;
            }
        }

        public async Task<int> GetScoreAsync(string playerId)
        {
            Player player = await FindPlayerAsync(playerId);
            if (player != null)
            {
                return player.Score;
            }
            else
            {
                return -1;
            }
        }

        public async Task<int> GetMoneyAsync(string playerId)
        {
            Player player = await FindPlayerAsync(playerId);
            if (player != null)
            {
                return player.Money;
            }
            else
            {
                return -1;
            }
        }

        // set functions
        public async Task<bool> SetIconIdAsync(string playerId, int iconId)
        {
            var player = await FindPlayerInRDSAsync(playerId);
            if (player == null) return false;

            player.IconId = iconId;
            await _context.SaveChangesAsync();

            string redisKey = $"player:{playerId}";
            await _database.HashSetAsync(redisKey, "IconId", iconId);

            return true;
        }

        public async Task<bool> SetScoreAsync(string playerId, int newScore)
        {
            var player = await FindPlayerInRDSAsync(playerId);
            if (player == null) return false;

            if(player.Score < newScore)
            {
                player.Score = newScore;
                await _context.SaveChangesAsync();

                string redisKey = $"player:{playerId}";
                await _database.HashSetAsync(redisKey, "Score", newScore);
            }

            return true;
        }

        public async Task<bool> SetMoneyAsync(string playerId, int money)
        {
            var player = await FindPlayerInRDSAsync(playerId);
            if (player == null) return false;

            player.Money = money;
            await _context.SaveChangesAsync();

            string redisKey = $"player:{playerId}";
            await _database.HashSetAsync(redisKey, "Money", money);

            return true;
        }
    }
}
