using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartyBot.Database.Entities
{
    [Table("GameInstance")]
    public class GameInstance : BaseEntity
    {
        public ulong? WinnerUserId { get; set; }

        [Required]
        public long GameId { get; set; }

        [Required]
        public long BoardId { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}
