using System.ComponentModel.DataAnnotations;

namespace ColorMemory.DTO
{
    public class MoneyDTO : InGameDTO
    {
        public int Money { get; set; }

        public MoneyDTO(string playerId, int money) : base(playerId)
        {
            Money = money;
        }
    }
}
