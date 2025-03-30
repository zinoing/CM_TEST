using System.ComponentModel.DataAnnotations;

namespace ColorMemory.DTO
{
    public class ScoreDTO : InGameDTO
    {
        public int Score { get; set; }

        public ScoreDTO(string playerId, int score) : base(playerId)
        {
            Score = score;
        }
    }
}
