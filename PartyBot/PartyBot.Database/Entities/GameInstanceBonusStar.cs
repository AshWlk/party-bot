using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartyBot.Database.Entities
{
    [Table("GameInstanceBonusStar")]
    public class GameInstanceBonusStar : BaseEntity
    {
        [Required]
        public int GameInstanceId { get; set; }

        [Required]
        public long BonusStarId { get; set; }

        [MaxLength(32)]
        public string? WinnerUserId { get; set; }

        [ForeignKey("GameInstanceId")]
        public virtual GameInstance? GameInstance { get; set; }
    }
}
