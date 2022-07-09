using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartyBot.Database.Entities
{
    [Table("GameInstance")]
    public class GameInstance : BaseEntity
    {
        public uint? WinnerUserId { get; set; }

        [Required]
        public Guid GameId { get; set; }

        [Required]
        public Guid BoardId { get; set; }
    }
}
