using System.ComponentModel.DataAnnotations;

namespace ColorMemory.DTO
{
    public class PlayerDTO : InGameDTO
    {
        [Required]
        public string Name { get; set; }
        public string IconId { get; set; }

        public int Score { get; set; }

        public int Money { get; set; }

        public PlayerDTO() { }

        public PlayerDTO(string playerId, string name, string iconId, int score, int money) : base(playerId)
        {
            Name = name;
            IconId = iconId;
            Score = score;
            Money = money;
        }
    }
}
