using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IMessagesProvider
    {
        string CreateStartMessage();

        string CreateMessagesDeletingError();

        string CreateEntityFormMessage(IEntity entity);

        string CreateEntityPhotosGuideMessage(IDisplayableEntity entity);

        string CreateEntityPhotosMessage(IDisplayableEntity entity, int[] selectedPhotoIndexes, int[] photoMessageIds);

        string CreateDeleteConfirmationMessage(IDisplayableEntity entity);

        string CreateDeletePhotosConfirmationMessage(IDisplayableEntity entity, int[] selectedPhotoIndexes);

        string CreateEntityListMessage<TEntity>(ListUnitActionMode actionMode, int totalEntitiesCount, IEntity? unitOwner = null) where TEntity : class, IDisplayableEntity;
    }
}