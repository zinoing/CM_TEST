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

    public class PlayerRankingDTO : ScoreDTO
    {
        public int IconId { get; set; }
        public string Name { get; set; }
        public int Ranking { get; set; }

        public PlayerRankingDTO() { }
        public PlayerRankingDTO(string playerId, int score, string name, int iconId, int ranking) : base(playerId, score)
        {
            IconId = iconId;
            Name = name;
            Score = score;
            Ranking = ranking;
        }
    }
}
