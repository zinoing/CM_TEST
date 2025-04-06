using Microsoft.EntityFrameworkCore;
using ColorMemory.Data;
using ColorMemory.Controllers;
using ColorMemory.DTO;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using System;

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

        private Dictionary<int, StageDTO> CalculateRankPerStage(Dictionary<int, StageDTO> stages)
        {
            for (int i = 1; i <= stages.Count; i++)
            {
                if (stages[i] == null) continue;

                // 만약 아직 클리어하지 못한 상태라면
                if (stages[i].Status == StageStauts.Lock)
                {
                    continue;
                }

                Rank newRank = Rank.NONE;

                // 다음 스테이지가 오픈될 경우엔 continue
                if(stages[i].Status == StageStauts.Open && stages[i].IncorrectCnt == -1 && stages[i].HintUsage == -1)
                {
                    continue;
                }

                if (stages[i].Status == StageStauts.Clear || stages[i].Status == StageStauts.Open)
                {
                    newRank = (stages[i].HintUsage + stages[i].IncorrectCnt) switch
                    {
                        <= 1 => Rank.GOLD,
                        <= 2 => Rank.SILVER,
                        _ => Rank.COPPER
                    };
                }

                stages[i].Rank = newRank;
            }

            return stages;
        }

        private Dictionary<int, StageDTO> UpdateStagesWithBetterPerformance(Dictionary<int, StageDTO> previousStages, Dictionary<int, StageDTO> newStages)
        {
            Dictionary<int, StageDTO> updatedStages = new Dictionary<int, StageDTO>();
            for (int i = 1; i <= previousStages.Count; i++)
            {
                // 만약 아직 잠겨 있는 상태라면
                if (newStages[i].Status == StageStauts.Lock)
                {
                    updatedStages.Add(i, previousStages[i]);
                    continue;
                }

                // 만약 열려 있는 상태라면
                if (newStages[i].Status == StageStauts.Open)
                {
                    updatedStages.Add(i, newStages[i]);
                    continue;
                }

                // 만약 이전에 플레이 해본 상태라면
                if (newStages[i].Status == StageStauts.Clear)
                {
                    if (previousStages[i].HintUsage == -1 && previousStages[i].IncorrectCnt == -1)
                    {
                        updatedStages.Add(i, newStages[i]);
                        continue;
                    }

                    if (previousStages[i].HintUsage + previousStages[i].IncorrectCnt > newStages[i].HintUsage + newStages[i].IncorrectCnt)
                    {
                        updatedStages.Add(i, newStages[i]);
                    }
                    else
                    {
                        updatedStages.Add(i, previousStages[i]);
                    }
                }
            }

            return updatedStages;
        }

        private Tuple<int, int, int> UpdateTotalMistakesAndHints(Dictionary<int, StageDTO> stages)
        {
            int totalMistakesAndHints = 0;
            int totalMistakes = 0;
            int totalHints = 0;
            for (int i = 1; i <= stages.Count; i++)
            {
                if (stages[i].Status == StageStauts.Lock) continue;
                if (stages[i].HintUsage == -1 && stages[i].IncorrectCnt == -1)
                {
                    continue;
                }

                totalMistakes += stages[i].IncorrectCnt;
                totalHints += stages[i].HintUsage;
            }
            totalMistakesAndHints = totalMistakes + totalHints;
            return new Tuple<int, int, int> ( totalMistakesAndHints, totalHints, totalMistakes );
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
            var updatedStage = UpdateStagesWithBetterPerformance(JsonConvert.DeserializeObject<Dictionary<int, StageDTO>>(existingEntry.Stages), playerArtworkInfo.Stages);
            updatedStage = CalculateRankPerStage(updatedStage);

            playerArtworkInfo.TotalMistakesAndHints = UpdateTotalMistakesAndHints(playerArtworkInfo.Stages).Item1;
            playerArtworkInfo.TotalHints = UpdateTotalMistakesAndHints(playerArtworkInfo.Stages).Item2;
            playerArtworkInfo.TotalMistakes = UpdateTotalMistakesAndHints(playerArtworkInfo.Stages).Item3;

            if (existingEntry.HasIt == false)
            {
                // 해당 artwork를 현재 player가 새롭게 가진 상태
                if (playerArtworkInfo.HasIt)
                {
                    updatedRank = playerArtworkInfo.TotalMistakesAndHints switch
                    {
                        <= 16 => Rank.GOLD,
                        <= 32 => Rank.SILVER,
                        _ => Rank.COPPER
                    };

                    existingEntry.Rank = updatedRank;
                    existingEntry.HasIt = true;
                    existingEntry.ObtainedDate = DateTime.Now;
                    existingEntry.TotalMistakesAndHints = playerArtworkInfo.TotalMistakesAndHints;
                    existingEntry.TotalHints = playerArtworkInfo.TotalHints;
                    existingEntry.TotalMistakes = playerArtworkInfo.TotalMistakes;
                    existingEntry.Stages = JsonConvert.SerializeObject(updatedStage);
                }
                // 여전히 가지지 못한 상태
                else
                {
                    existingEntry.Rank = Rank.NONE;
                    existingEntry.HasIt = false;
                    existingEntry.ObtainedDate = null;
                    existingEntry.TotalMistakesAndHints = playerArtworkInfo.TotalMistakesAndHints;
                    existingEntry.TotalHints = playerArtworkInfo.TotalHints;
                    existingEntry.TotalMistakes = playerArtworkInfo.TotalMistakes;
                    existingEntry.Stages = JsonConvert.SerializeObject(updatedStage);
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
                    updatedRank = playerArtworkInfo.TotalMistakesAndHints switch
                    {
                        <= 16 => Rank.GOLD,
                        <= 32 => Rank.SILVER,
                        _ => Rank.COPPER
                    };

                    existingEntry.Rank = updatedRank;
                    existingEntry.HasIt = true;
                    existingEntry.ObtainedDate = DateTime.Now;
                    existingEntry.TotalMistakesAndHints = playerArtworkInfo.TotalMistakesAndHints;
                    existingEntry.TotalHints = playerArtworkInfo.TotalHints;
                    existingEntry.TotalMistakes = playerArtworkInfo.TotalMistakes;
                    existingEntry.Stages = JsonConvert.SerializeObject(updatedStage);
                }
                else
                {
                    existingEntry.HasIt = true;
                    existingEntry.TotalMistakesAndHints = playerArtworkInfo.TotalMistakesAndHints;
                    existingEntry.TotalHints = playerArtworkInfo.TotalHints;
                    existingEntry.TotalMistakes = playerArtworkInfo.TotalMistakes;
                    existingEntry.Stages = JsonConvert.SerializeObject(updatedStage);
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
                    totalHints: pa.TotalHints,
                    totalMistakes: pa.TotalMistakes,
                    stages: JsonConvert.DeserializeObject<Dictionary<int, StageDTO>>(pa.Stages),
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
                    totalMistakesAndHints: pa.TotalMistakesAndHints,
                    totalHints: pa.TotalHints,
                    totalMistakes: pa.TotalMistakes,
                    stages: JsonConvert.DeserializeObject<Dictionary<int, StageDTO>>(pa.Stages),
                    rank: pa.Rank,
                    hasIt: pa.HasIt,
                    obtainedDate: null
                );

                result.Add(dto);
            }

            return result;
        }
        public async Task<List<PlayerArtworkDTO>> GetWholePlayerArtworksAsync(string playerId)
        {
            var player = await _context.Players
                .Include(p => p.PlayerArtworks)
                    .ThenInclude(pa => pa.Artwork)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);

            if (player == null)
                return new List<PlayerArtworkDTO>();

            var result = new List<PlayerArtworkDTO>();

            foreach (var pa in player.PlayerArtworks)
            {
                var dto = new PlayerArtworkDTO(
                    playerId: playerId,
                    artworkId: pa.Artwork.ArtworkId,
                    title: pa.Artwork.Title,
                    artist: pa.Artwork.Artist,
                    totalMistakesAndHints: 0,
                    totalHints: pa.TotalHints,
                    totalMistakes: pa.TotalMistakes,
                    stages: JsonConvert.DeserializeObject<Dictionary<int, StageDTO>>(pa.Stages),
                    rank: pa.Rank,
                    hasIt: pa.HasIt,
                    obtainedDate: pa.ObtainedDate
                );

                result.Add(dto);
            }

            return result;
        }
    }

}