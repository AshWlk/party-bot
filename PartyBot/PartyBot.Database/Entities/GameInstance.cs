using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartyBot.Database.Entities
{
    [Table("GameInstance")]
    public class GameInstance : BaseEntity
    {
        [MaxLength(32)]
        public string? WinnerUserId { get; set; }

        [Required]
        public long GameId { get; set; }

        [Required]
        public long BoardId { get; set; }

        public DateTimeOffset Date { get; set; }

        [InverseProperty("GameInstance")]
        public virtual ICollection<GameInstanceBonusStar> GameInstanceBonusStars { get; set; } = new HashSet<GameInstanceBonusStar>();
    }
}
