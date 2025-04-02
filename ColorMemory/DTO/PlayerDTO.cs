using System.ComponentModel.DataAnnotations;

namespace ColorMemory.DTO
{
    public class PlayerDTO : InGameDTO
    {
        [Required]
        public string Name { get; set; }
        public int IconId { get; set; }

        public int Score { get; set; }

        public int Money { get; set; }

        public PlayerDTO() { }

        public PlayerDTO(string playerId, string name, int iconId = 0, int score = 0, int money = 0) : base(playerId)
        {
            Name = name;
            IconId = iconId;
            Score = score;
            Money = money;
        }
    }
}
