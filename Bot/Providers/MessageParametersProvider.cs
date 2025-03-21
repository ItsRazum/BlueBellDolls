using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Providers
{
    public class MessageParametersProvider : IMessageParametersProvider
    {
        private readonly IMessagesProvider _messagesProvider;
        private readonly IKeyboardsProvider _keyboardsProvider;

        public MessageParametersProvider(
            IMessagesProvider messagesProvider,
            IKeyboardsProvider keyboardsProvider)
        {
            _messagesProvider = messagesProvider;
            _keyboardsProvider = keyboardsProvider;
        }

        public MessageParameters GetEntityFormParameters(IDisplayableEntity entity)
        {
            return new MessageParameters(
                _messagesProvider.CreateEntityFormMessage(entity),
                _keyboardsProvider.CreateEntityOptionsKeyboard(entity)
                );
        }

        public MessageParameters GetEntityPhotosParameters(IDisplayableEntity entity, int[] selectedPhotoIndexes, int[] photoMessageIds)
        {
            return new MessageParameters(
                _messagesProvider.CreateEntityPhotosMessage(entity, selectedPhotoIndexes, photoMessageIds),
                _keyboardsProvider.CreateEntityPhotosKeyboard(entity, photoMessageIds, selectedPhotoIndexes));
        }

        public MessageParameters GetDeleteEntityPhotosConfirmationParameters(IDisplayableEntity entity, string callback, (int[] selectedPhotoIndexes, string[] selectedPhotoFileIds) photoParameters, string onDeletionCanceledCallback, params string[] callbacksAfterDeletion)
        {
            var inputFiles = photoParameters.selectedPhotoFileIds.Select(p => new InputMediaPhoto(new InputFileId(p)));
            return new MessageParameters(
                _messagesProvider.CreateDeletePhotosConfirmationMessage(entity, photoParameters.selectedPhotoIndexes),
                _keyboardsProvider.CreateYesNoKeyboard(callback, entity, onDeletionCanceledCallback, callbacksAfterDeletion));
        }

        public MessageParameters GetDeleteEntityConfirmationParameters(IDisplayableEntity entity, string callback, string onDeletionCanceledCallback, params string[] callbacksAfterDeletion)
        {
            return new MessageParameters(
                _messagesProvider.CreateDeleteConfirmationMessage(entity),
                _keyboardsProvider.CreateYesNoKeyboard(callback, entity, onDeletionCanceledCallback, callbacksAfterDeletion));
        }

        public MessageParameters GetEntityListParameters<TEntity>(
            IEnumerable<TEntity> entities, 
            ListUnitActionMode actionMode, 
            (int page, int totalPagesCount, int totalEntitiesCount) pageParameters,
            IEntity? unitOwner = null) 
            where TEntity : class, IDisplayableEntity
        {
            return new MessageParameters(
                _messagesProvider.CreateEntityListMessage<TEntity>(actionMode, pageParameters.totalEntitiesCount),
                _keyboardsProvider.CreateEntityListKeyboard(entities, actionMode, (pageParameters.page, pageParameters.totalPagesCount), unitOwner));
        }

        public MessageParameters GetEntityFromLitterParameters(IDisplayableEntity entity, int litterId)
        {
            return new MessageParameters(
                _messagesProvider.CreateEntityFormMessage(entity),
                _keyboardsProvider.CreateEntityFromLitterKeyboard(entity, litterId));
        }
    }
}
