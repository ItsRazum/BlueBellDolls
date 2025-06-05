using BlueBellDolls.Common.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BlueBellDolls.Common.Types
{
    public abstract class EntityBase : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual bool IsEnabled { get; set; }
    }
}
