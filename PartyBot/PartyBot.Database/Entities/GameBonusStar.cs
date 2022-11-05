using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartyBot.Database.Entities
{
    [Table("GameBonusStar")]
    public class GameBonusStar : BaseEntity
    {
        [Required]
        public int GameId { get; set; }

        [Required]
        public int BonusStarId { get; set; }

        [ForeignKey("GameId")]
        public Game? Game { get; set; }

        [ForeignKey("BonusStarId")]
        public BonusStar? BonusStar { get; set; }

    }
}
