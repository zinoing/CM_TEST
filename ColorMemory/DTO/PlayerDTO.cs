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

        public PlayerDTO(string playerId, string name, string iconId = "default", int score = 0, int money = 0) : base(playerId)
        {
            Name = name;
            IconId = iconId;
            Score = score;
            Money = money;
        }
    }
}
