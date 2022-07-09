using System.ComponentModel.DataAnnotations;

namespace PartyBot.Database.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
