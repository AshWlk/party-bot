using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartyBot.Database.Entities
{
    [Table("GameInstanceBonusStar")]
    public class GameInstanceBonusStar : BaseEntity
    {
        [Required]
        public Guid BonusStarId { get; set; }

        public uint? WinnerUserId { get; set; }
    }
}
