using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartyBot.Database.Entities
{
    [Table("GameInstance")]
    public class GameInstance : BaseEntity
    {
        public ulong? WinnerUserId { get; set; }

        [Required]
        public int GameId { get; set; }

        [Required]
        public int BoardId { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}
