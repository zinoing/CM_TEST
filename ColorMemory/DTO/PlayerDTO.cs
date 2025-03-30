using System.ComponentModel.DataAnnotations;

namespace ColorMemory.DTO
{
    public class PlayerDTO : InGameDTO
    {
        [Required]
        public string Name { get; set; }

        public int Score { get; set; }

        public int Money { get; set; }

        public PlayerDTO(string playerId, string name, int score, int money) : base(playerId)
        {
            Name = name;
            Score = score;
            Money = money;
        }
    }
}
