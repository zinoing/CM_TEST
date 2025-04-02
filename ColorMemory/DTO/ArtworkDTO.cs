using ColorMemory.Data;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace ColorMemory.DTO
{
    public class ArtworkDTO : InGameDTO
    {
        public int ArtworkId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Artist { get; set; }

        public ArtworkDTO() { }

        public ArtworkDTO(string playerId, int artworkId, string title, string artist) : base(playerId)
        {
            ArtworkId = artworkId;
            Title = title;
            Artist = artist;
        }
    }

    public class MoveDTO
    {
        public int Stage { get; set; }
        public int MoveCount { get; set; }

        public MoveDTO() { }

        public MoveDTO(int stage, int moveCount)
        {
            Stage = stage;
            MoveCount = moveCount;
        }
    }

    public enum Rank
    {
        NONE,
        COPPER,
        SILVER,
        GOLD,
    }

    public class StageDTO
    {
        public Rank Rank { get; set; }
        public int HintUsage { get; set; }
        public int IncorrectCnt { get; set; }

        public StageDTO() { }

        public StageDTO(Rank rank, int hintUsage, int incorrectCnt)
        {
            Rank = rank;
            HintUsage = hintUsage;
            IncorrectCnt = incorrectCnt;
        }
    }

    public class PlayerArtworkDTO : ArtworkDTO
    {
        public int TotalMistakesAndHints { get; set; }

        public Dictionary<int, StageDTO> Stages { get; set; }

        public Rank Rank { get; set; }
        public bool HasIt { get; set; }
        public DateTime? ObtainedDate { get; set; }

        public PlayerArtworkDTO() { }

        public PlayerArtworkDTO(string playerId, int artworkId, string title, string artist, int totalMistakesAndHints, Dictionary<int, StageDTO> stages, Rank rank, bool hasIt, DateTime? obtainedDate)
            : base(playerId, artworkId, title, artist)
        {
            TotalMistakesAndHints = totalMistakesAndHints;
            Stages = stages;
            Rank = rank;
            HasIt = hasIt;
            ObtainedDate = obtainedDate;
        }

        public PlayerArtworkDTO(ArtworkDTO artworkDTO, int totalMistakesAndHints, Dictionary<int, StageDTO> stages, Rank rank, bool hasIt, DateTime? obtainedDate)
        : base(artworkDTO.PlayerId, artworkDTO.ArtworkId, artworkDTO.Title, artworkDTO.Artist)
        {
            TotalMistakesAndHints = totalMistakesAndHints;
            Stages = stages;
            Rank = rank;
            HasIt = hasIt;
            ObtainedDate = obtainedDate;
        }
    }
}
