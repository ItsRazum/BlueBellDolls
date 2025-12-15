using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Common.Types
{
    public abstract class EntityBase : IEntity
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; } = DateTime.Now;
    }
}
