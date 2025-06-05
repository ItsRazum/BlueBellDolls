using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Common.Types
{
    public abstract class DisplayableEntityBase : EntityBase, IDisplayableEntity
    {

        public abstract string DisplayName { get; }

        public virtual Dictionary<string, string> Photos { get; set; }

    }
}
