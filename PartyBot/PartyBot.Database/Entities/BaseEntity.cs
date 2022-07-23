using System.ComponentModel.DataAnnotations;

namespace PartyBot.Database.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
