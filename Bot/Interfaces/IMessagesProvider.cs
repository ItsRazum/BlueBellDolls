using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IMessagesProvider
    {
        string CreateStartMessage();

        string CreateEntityFormMessage(IEntity entity);

        string CreateDeleteConfirmationMessage(IDisplayableEntity entity);

        string CreateEntityListMessage<TEntity>(ListUnitActionMode actionMode, int totalEntitiesCount, IEntity? unitOwner = null) where TEntity : class, IDisplayableEntity;
    }
}