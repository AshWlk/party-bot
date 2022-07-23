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
        public int BonusStarId { get; set; }

        public ulong? WinnerUserId { get; set; }
    }
}
