using System.ComponentModel.DataAnnotations;

namespace ColorMemory.DTO
{
    public class InGameDTO
    {
        [Required]
        public string PlayerId { get; set; }

        public InGameDTO() { }

        public InGameDTO(string playerId)
        {
            PlayerId = playerId;
        }
    }
}
