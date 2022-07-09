using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartyBot.Database.Entities
{
    [Table("BonusStar")]
    public class BonusStar : BaseEntity
    {
        [Required]
        [MaxLength(64)]
        public string Name { get; set; }
    }
}
