using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Types
{
    public abstract class DisplayableEntityBase : EntityBase, IDisplayableEntity
    {

        public abstract string DisplayName { get; }

        public virtual List<EntityPhoto> Photos { get; set; }

        [Column("is_enabled")]
        public virtual bool IsEnabled { get; set; }

        public abstract bool ReadyToShow { get; }
    }
}
