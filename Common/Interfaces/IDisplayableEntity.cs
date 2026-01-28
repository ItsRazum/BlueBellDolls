using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Common.Interfaces
{
    public interface IDisplayableEntity : IEntity
    {
        string DisplayName { get; }

        List<EntityPhoto> Photos { get; set; }

        bool IsEnabled { get; set; }

        bool ReadyToShow { get; }
    }
}
