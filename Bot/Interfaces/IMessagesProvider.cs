using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IMessagesProvider
    {
        string CreateStartMessage();

        string CreateMessagesDeletingError();

        string CreateEntityUpdateSuccessMessage();
        
        string CreateEntityUpdateFailureMessage();

        string CreateEntityNotFoundMessage();

        string CreateEntityNotFoundMessage(Type entityType, int entityId);

        string CreateEntityFormMessage(IEntity entity);

        string CreateEntityPhotosGuideMessage(IDisplayableEntity entity);

        string CreatePhotosLoadingMessage();

        string CreatePhotosLimitationErrorMessage();

        string CreateEntityPhotosMessage(IDisplayableEntity entity, int[] selectedPhotoIndexes, int[] photoMessageIds);

        string CreateDeleteConfirmationMessage(IDisplayableEntity entity);

        string CreateEntityDeletionSuccess();

        string CreateDeletePhotosConfirmationMessage(IDisplayableEntity entity, int[] selectedPhotoIndexes);

        string CreateEntityListMessage<TEntity>(ListUnitActionMode actionMode, int totalEntitiesCount, IEntity? unitOwner = null) where TEntity : class, IDisplayableEntity;

        string CreateCouldNotExtractMessagesFromCallbackMessage(CallbackQueryAdapter callback);

        string CreateParentCatSetForLitter(ParentCat parentCat, Litter litter);

        string CreateDefaultPhotoSetForEntityMessage(IDisplayableEntity entity, int photoIndex);
    }
}