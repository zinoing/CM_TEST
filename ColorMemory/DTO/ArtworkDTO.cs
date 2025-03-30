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

    public class PlayerArtworkDTO : ArtworkDTO
    {
        public int TotalMoveCount { get; set; }

        public Dictionary<int, int> Moves { get; set; }

        public Rank Rank { get; set; }

        public bool HasIt { get; set; }

        public PlayerArtworkDTO() { }

        public PlayerArtworkDTO(string playerId, int artworkId, string title, string artist, int totalMoveCount, Dictionary<int, int> moves, Rank rank, bool hasIt)
            : base(playerId, artworkId, title, artist)
        {
            TotalMoveCount = totalMoveCount;
            Moves = moves;
            Rank = rank;
            HasIt = hasIt;
        }

        public PlayerArtworkDTO(ArtworkDTO artworkDTO, int totalMoveCount, Dictionary<int, int> moves, Rank rank, bool hasIt)
        : base(artworkDTO.PlayerId, artworkDTO.ArtworkId, artworkDTO.Title, artworkDTO.Artist)
        {
            TotalMoveCount = totalMoveCount;
            Moves = moves;
            Rank = rank;
            HasIt = hasIt;
        }
    }
}
