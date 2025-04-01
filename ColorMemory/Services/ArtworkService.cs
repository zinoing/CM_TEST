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

        public async Task<Rank?> ObtainPlayerArtworkAsync(PlayerArtworkDTO playerArtworkInfo)
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

            Rank newRank = Rank.NONE;

            if (existingEntry == null)
            {
                // 아직 이 아트워크를 아예 안 가진 상태 — 새로 추가
                newRank = playerArtworkInfo.TotalMistakesAndHints switch
                {
                    <= 16 => Rank.GOLD,
                    <= 32 => Rank.SILVER,
                    _ => Rank.COPPER
                };

                var newPlayerArtwork = new PlayerArtwork
                {
                    PlayerId = playerId,
                    ArtworkId = artworkId,
                    Rank = newRank,
                    HasIt = true,
                    ObtainedDate = DateTime.Now,
                    TotalMistakesAndHints = playerArtworkInfo.TotalMistakesAndHints,
                    HintUsagePerStage = JsonConvert.SerializeObject(playerArtworkInfo.HintUsagePerStage),
                    IncorrectPerStage = JsonConvert.SerializeObject(playerArtworkInfo.IncorrectPerStage),
                };

                player.PlayerArtworks.Add(newPlayerArtwork);
            }
            else if (!existingEntry.HasIt)
            {
                // 이미 같은 키의 객체가 있으나 HasIt이 false인 경우 → 값만 업데이트
                newRank = playerArtworkInfo.TotalMistakesAndHints switch
                {
                    <= 16 => Rank.GOLD,
                    <= 32 => Rank.SILVER,
                    _ => Rank.COPPER
                };

                existingEntry.Rank = newRank;
                existingEntry.HasIt = true;
                existingEntry.ObtainedDate = DateTime.Now;
                existingEntry.TotalMistakesAndHints = playerArtworkInfo.TotalMistakesAndHints;
                existingEntry.HintUsagePerStage = JsonConvert.SerializeObject(playerArtworkInfo.HintUsagePerStage);
                existingEntry.IncorrectPerStage = JsonConvert.SerializeObject(playerArtworkInfo.IncorrectPerStage);
            }

            await _context.SaveChangesAsync();
            return newRank;
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

            if (existingEntry == null) return null;

            Rank updatedRank = Rank.NONE;

            if (existingEntry.HasIt == false)
            {
                updatedRank = playerArtworkInfo.TotalMistakesAndHints switch
                {
                    <= 16 => Rank.GOLD,
                    <= 32 => Rank.SILVER,
                    _ => Rank.COPPER
                };

                // 해당 artwork를 현재 player가 새롭게 가진 상태
                if (playerArtworkInfo.HasIt)
                {
                    existingEntry.Rank = updatedRank;
                    existingEntry.HasIt = true;
                    existingEntry.ObtainedDate = DateTime.Now;
                    existingEntry.TotalMistakesAndHints = playerArtworkInfo.TotalMistakesAndHints;
                    existingEntry.HintUsagePerStage = JsonConvert.SerializeObject(playerArtworkInfo.HintUsagePerStage);
                    existingEntry.IncorrectPerStage = JsonConvert.SerializeObject(playerArtworkInfo.IncorrectPerStage);
                }
                // 여전히 가지지 못한 상태
                else
                {
                    existingEntry.Rank = updatedRank;
                    existingEntry.HasIt = false;
                    existingEntry.ObtainedDate = null;
                    existingEntry.TotalMistakesAndHints = playerArtworkInfo.TotalMistakesAndHints;
                    existingEntry.HintUsagePerStage = JsonConvert.SerializeObject(playerArtworkInfo.HintUsagePerStage);
                    existingEntry.IncorrectPerStage = JsonConvert.SerializeObject(playerArtworkInfo.IncorrectPerStage);
                }
            }
            else {
                // 이미 획득한 artwork라면 랭크 재계산
                updatedRank = playerArtworkInfo.TotalMistakesAndHints switch
                {
                    <= 16 => Rank.GOLD,
                    <= 32 => Rank.SILVER,
                    _ => Rank.COPPER
                };

                // 랭크가 변화되었다면 갱신
                if(updatedRank > existingEntry.Rank)
                {
                    existingEntry.Rank = updatedRank;
                    existingEntry.HasIt = true;
                    existingEntry.ObtainedDate = DateTime.Now;
                    existingEntry.TotalMistakesAndHints = playerArtworkInfo.TotalMistakesAndHints;
                    existingEntry.HintUsagePerStage = JsonConvert.SerializeObject(playerArtworkInfo.HintUsagePerStage);
                    existingEntry.IncorrectPerStage = JsonConvert.SerializeObject(playerArtworkInfo.IncorrectPerStage);
                }
                else
                {
                    existingEntry.HasIt = true;
                    existingEntry.TotalMistakesAndHints = playerArtworkInfo.TotalMistakesAndHints;
                    existingEntry.HintUsagePerStage = JsonConvert.SerializeObject(playerArtworkInfo.HintUsagePerStage);
                    existingEntry.IncorrectPerStage = JsonConvert.SerializeObject(playerArtworkInfo.IncorrectPerStage);
                }
            }
            await _context.SaveChangesAsync();

            return updatedRank;
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
                    totalMistakesAndHints: pa.TotalMistakesAndHints,
                    hintUsagePerStage: JsonConvert.DeserializeObject<Dictionary<int, int>>(pa.HintUsagePerStage),
                    incorrectPerStage: JsonConvert.DeserializeObject<Dictionary<int, int>>(pa.IncorrectPerStage),
                    rank: pa.Rank,
                    hasIt: pa.HasIt,
                    obtainedDate: pa.ObtainedDate
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
                    totalMistakesAndHints: 0,
                    hintUsagePerStage: JsonConvert.DeserializeObject<Dictionary<int, int>>(pa.HintUsagePerStage),
                    incorrectPerStage: JsonConvert.DeserializeObject<Dictionary<int, int>>(pa.IncorrectPerStage),
                    rank: pa.Rank,
                    hasIt: pa.HasIt,
                    obtainedDate: null
                );

                result.Add(dto);
            }

            return result;
        }

    }

}