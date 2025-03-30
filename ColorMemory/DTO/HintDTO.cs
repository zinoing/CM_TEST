using System.ComponentModel.DataAnnotations;

namespace ColorMemory.DTO
{
    public enum HintType
    {
        OneColorHint,
        OneZoneHint
    }

    public class HintDTO : InGameDTO
    {
        public HintType Type { get; set; }
        public HintDTO(string playerId, HintType type) : base(playerId)
        {
            Type = type;
        }
    }
}
