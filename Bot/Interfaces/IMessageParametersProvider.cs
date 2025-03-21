using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IMessageParametersProvider
    {
        MessageParameters GetEntityFormParameters(IDisplayableEntity entity);
        MessageParameters GetEntityPhotosParameters(IDisplayableEntity entity, int[] selectedPhotoIndexes, int[] photoMessageIds);
        MessageParameters GetDeleteEntityConfirmationParameters(
            IDisplayableEntity entity, 
            string callback, 
            string onDeletionCanceledCallback, 
            params string[] callbacksAfterDeletion);

        MessageParameters GetDeleteEntityPhotosConfirmationParameters(
            IDisplayableEntity entity, 
            string callback,
            (int[] selectedPhotoIndexes, string[] selectedPhotoFileIds) photoParameters,
            string onDeletionCanceledCallback, 
            params string[] callbacksAfterDeletion);

        MessageParameters GetEntityListParameters<TEntity>(
            IEnumerable<TEntity> entities, 
            ListUnitActionMode actionMode, 
            (int page, int totalPagesCount, int totalEntitiesCount) pageParameters,
            IEntity? unitOwner = null)
            where TEntity : class, IDisplayableEntity;

        MessageParameters GetEntityFromLitterParameters(IDisplayableEntity entity, int litterId);
    }
}