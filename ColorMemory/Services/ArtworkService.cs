using Microsoft.EntityFrameworkCore;
using ColorMemory.Data;
using ColorMemory.Controllers;
using ColorMemory.DTO;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;

namespace ColorMemory.Services
{
    public class ArtworkService
    {
        private readonly GameDbContext _context;

        public ArtworkService(GameDbContext context, IConfiguration configuration)
        {
            _context = context;
        }

        public async Task<Artwork> AddArtworkAsync(string fileName)
        {
            if (fileName.EndsWith(".json"))
            {
                fileName = fileName[..^5];
            }

            if (fileName.EndsWith(".jpg"))
            {
                fileName = fileName[..^4];
            }

            var existingArtwork = await _context.Artworks
                .FirstOrDefaultAsync(a => a.FileName == fileName);

            if (existingArtwork != null)
            {
                return null;
            }

            int byIndex = fileName.IndexOf(" by ");
            if (byIndex == -1)
            {
                throw new ArgumentException("Invalid file name format. Expected format: '<Title> by <Artist>.json'");
            }

            string title = fileName[..byIndex].Trim();
            string artist = fileName[(byIndex + 4)..].Trim();
            var artwork = new Artwork
            {
                Title = title,
                Artist = artist,
                FileName = fileName,
            };

            _context.Artworks.Add(artwork);
            await _context.SaveChangesAsync();

            return artwork;
        }

        public async Task<Rank?> UpdatePlayerArtworkAsync(PlayerArtworkDTO playerArtworkInfo)
        {
            int artworkId = playerArtworkInfo.ArtworkId;
            string playerId = playerArtworkInfo.PlayerId;

            var player = await _context.Players
                .Include(p => p.PlayerArtworks)
                .ThenInclude(pa => pa.Artwork)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null)
                return null;

            var artwork = await _context.Artworks.FindAsync(artworkId);
            if (artwork == null)
                return null;

            var existingEntry = player.PlayerArtworks
                .FirstOrDefault(pa => pa.ArtworkId == artworkId);

            Rank newRank;

            if (existingEntry == null)
            {
                newRank = Rank.NONE;
                var newPlayerArtwork = new PlayerArtwork
                {
                    PlayerId = playerId,
                    ArtworkId = artworkId,
                    Rank = newRank,
                    HasIt = true,
                    TotalMoveCnt = playerArtworkInfo.TotalMoveCount,
                    Moves = JsonConvert.SerializeObject(playerArtworkInfo.Moves)
                };

                player.PlayerArtworks.Add(newPlayerArtwork);
            }
            else
            {
                if(playerArtworkInfo.HasIt)
                {
                    newRank = playerArtworkInfo.TotalMoveCount switch
                    {
                        <= 16 => Rank.GOLD,
                        <= 32 => Rank.SILVER,
                        _ => Rank.COPPER
                    };

                    existingEntry.Rank = newRank;
                }
                else
                {
                    newRank = playerArtworkInfo.Rank;
                }
                
                existingEntry.HasIt = playerArtworkInfo.HasIt;
                existingEntry.TotalMoveCnt = playerArtworkInfo.TotalMoveCount;
                existingEntry.Moves = JsonConvert.SerializeObject(playerArtworkInfo.Moves);
            }

            await _context.SaveChangesAsync();
            return newRank;
        }

        public async Task<List<PlayerArtworkDTO>> GetOwnedArtworksAsync(string playerId)
        {
            var player = await _context.Players
                .Include(p => p.PlayerArtworks)
                    .ThenInclude(pa => pa.Artwork)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null)
                return new List<PlayerArtworkDTO>();

            var result = new List<PlayerArtworkDTO>();

            foreach (var pa in player.PlayerArtworks.Where(pa => pa.HasIt))
            {
                var dto = new PlayerArtworkDTO(
                    playerId: pa.PlayerId,
                    artworkId: pa.Artwork.ArtworkId,
                    title: pa.Artwork.Title,
                    artist: pa.Artwork.Artist,
                    totalMoveCount: pa.TotalMoveCnt,
                    moves: JsonConvert.DeserializeObject<Dictionary<int, int>>(pa.Moves),
                    rank: pa.Rank,
                    hasIt: pa.HasIt
                );

                result.Add(dto);
            }

            return result;
        }

        public async Task<List<PlayerArtworkDTO>> GetUnownedArtworksAsync(string playerId)
        {
            var player = await _context.Players
                .Include(p => p.PlayerArtworks)
                    .ThenInclude(pa => pa.Artwork)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null)
                return new List<PlayerArtworkDTO>();

            var result = new List<PlayerArtworkDTO>();

            foreach (var pa in player.PlayerArtworks.Where(pa => !pa.HasIt))
            {
                var dto = new PlayerArtworkDTO(
                    playerId: playerId,
                    artworkId: pa.Artwork.ArtworkId,
                    title: pa.Artwork.Title,
                    artist: pa.Artwork.Artist,
                    totalMoveCount: 0,
                    moves: JsonConvert.DeserializeObject<Dictionary<int, int>>(pa.Moves),
                    rank: pa.Rank,
                    hasIt: pa.HasIt
                );

                result.Add(dto);
            }

            return result;
        }

    }

}