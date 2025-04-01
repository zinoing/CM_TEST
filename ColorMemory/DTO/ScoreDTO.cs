using System.ComponentModel.DataAnnotations;

namespace ColorMemory.DTO
{
    public class ScoreDTO : InGameDTO
    {
        public int Score { get; set; }

        public ScoreDTO() { }
        public ScoreDTO(string playerId, int score) : base(playerId)
        {
            Score = score;
        }
    }

    public class PlayerScoreDTO : ScoreDTO
    {
        public string IconId { get; set; }
        public string Name { get; set; }

        public PlayerScoreDTO() { }
        public PlayerScoreDTO(string playerId, string name, string iconId, int score) : base(playerId, score)
        {
            IconId = iconId;
            Name = name;
            Score = score;
        }
    }
}
