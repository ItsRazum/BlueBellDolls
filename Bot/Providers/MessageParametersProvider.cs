﻿using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Types;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Providers
{
    public class MessageParametersProvider(
        IMessagesProvider messagesProvider,
        IKeyboardsProvider keyboardsProvider) : IMessageParametersProvider
    {
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly IKeyboardsProvider _keyboardsProvider = keyboardsProvider;

        public MessageParameters GetStartParameters()
            => new MessageParameters(
                _messagesProvider.CreateStartMessage(),
                _keyboardsProvider.CreateStartKeyboard());

        public MessageParameters GetEntityFormParameters(IDisplayableEntity entity)
        {
            return new MessageParameters(
                _messagesProvider.CreateEntityFormMessage(entity),
                _keyboardsProvider.CreateEntityOptionsKeyboard(entity),
                [..entity.Photos.Take(1).Select(p => new InputMediaPhoto(p.Key))]
                );
        }

        public MessageParameters GetEntityPhotosParameters(
            IDisplayableEntity entity, 
            PhotosManagementMode photosManagementMode, 
            int[] selectedPhotoIndexes,
            int[] photoMessageIds)
        {
            return new MessageParameters(
                _messagesProvider.CreateEntityPhotosMessage(entity, selectedPhotoIndexes, photoMessageIds),
                _keyboardsProvider.CreateEntityPhotosKeyboard(entity, photosManagementMode, photoMessageIds, selectedPhotoIndexes));
        }

        public MessageParameters GetDeleteEntityPhotosConfirmationParameters(
            IDisplayableEntity entity,
            string callback,
            int[] selectedPhotoIndexes,
            int[] sendedPhotoMessageIds,
            string onDeletionCanceledCallback,
            params string[] callbacksAfterDeletion)
        {
            return new MessageParameters(
                _messagesProvider.CreateDeletePhotosConfirmationMessage(entity, selectedPhotoIndexes, sendedPhotoMessageIds),
                _keyboardsProvider.CreateYesNoKeyboard(callback, entity, onDeletionCanceledCallback, callbacksAfterDeletion));
        }

        public MessageParameters GetDeleteEntityConfirmationParameters(
            IDisplayableEntity entity,
            string callback,
            string onDeletionCanceledCallback,
            params string[] callbacksAfterDeletion)
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
                _keyboardsProvider.CreateEntityListKeyboard(entities, actionMode, 1, (pageParameters.page, pageParameters.totalPagesCount), unitOwner));
        }

        public MessageParameters GetEntityFromLitterParameters(IDisplayableEntity entity, int litterId)
        {
            return new MessageParameters(
                _messagesProvider.CreateEntityFormMessage(entity, false),
                _keyboardsProvider.CreateEntityFromLitterKeyboard(entity, litterId),
                [.. entity.Photos.Take(1).Select(p => new InputMediaPhoto(p.Key))]);
        }

        public MessageParameters GetColorPickerParameters(Cat cat, string buildedColor, string[] findedColorParts)
        {
            return new MessageParameters(
                _messagesProvider.CreateColorPickerMessage(cat, buildedColor),
                _keyboardsProvider.CreateColorPickerKeyboard(cat, buildedColor, findedColorParts));
        }
    }
}
