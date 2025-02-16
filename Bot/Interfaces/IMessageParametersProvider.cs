using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IMessageParametersProvider
    {
        MessageParameters GetEntityMessageParameters(IDisplayableEntity entity);
        MessageParameters GetDeleteConfirmationParameters(string callback, IDisplayableEntity entity, string onDeletionCanceledCallback, params string[] callbacksAfterDeletion);
        MessageParameters GetEntityListParameters<TEntity>(
            IEnumerable<TEntity> entities, 
            ListUnitActionMode actionMode, 
            (int page, int totalPagesCount, int totalEntitiesCount) pageParameters,
            IEntity? unitOwner = null)
            where TEntity : class, IDisplayableEntity;
        MessageParameters GetEntityFromLitterParameters(IDisplayableEntity entity, int litterId);
    }
}