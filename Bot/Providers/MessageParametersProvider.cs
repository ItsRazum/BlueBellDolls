using BlueBellDolls.Bot.Enums;
using Telegram.Bot.Types;
using BlueBellDolls.Common.Types;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;

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
                [..entity.Photos.Where(p => p.Type == PhotosType.Photos).Take(1).Select(p => new InputMediaPhoto(p.TelegramPhoto?.FileId ?? string.Empty))]
                );
        }

        public MessageParameters GetEntityPhotosParameters(
            IDisplayableEntity entity, 
            PhotosType photosType, 
            int[] selectedPhotoIds,
            int[] photoMessageIds)
        {
            return new MessageParameters(
                _messagesProvider.CreateEntityPhotosMessage(entity, selectedPhotoIds, photoMessageIds),
                _keyboardsProvider.CreateEntityPhotosKeyboard(entity, photosType, photoMessageIds, selectedPhotoIds));
        }

        public MessageParameters GetDeleteEntityPhotosConfirmationParameters(
            IDisplayableEntity entity,
            string callback,
            int[] selectedPhotoIds,
            int[] sendedPhotoMessageIds,
            string onDeletionCanceledCallback)
        {
            return new MessageParameters(
                _messagesProvider.CreateDeletePhotosConfirmationMessage(entity, selectedPhotoIds, sendedPhotoMessageIds),
                _keyboardsProvider.CreateYesNoKeyboard(callback, entity, onDeletionCanceledCallback));
        }

        public MessageParameters GetDeleteEntityConfirmationParameters(
            IDisplayableEntity entity,
            string callback,
            string onDeletionCanceledCallback)
        {
            return new MessageParameters(
                _messagesProvider.CreateDeleteConfirmationMessage(entity),
                _keyboardsProvider.CreateYesNoKeyboard(callback, entity, onDeletionCanceledCallback));
        }

        public MessageParameters GetEntityListParameters<TEntity>(
            IEnumerable<TEntity> entities, 
            ListUnitActionMode actionMode, 
            (int page, int totalPagesCount, int totalEntitiesCount) pageParameters,
            int? litterOwnerId = null) 
            where TEntity : class, IDisplayableEntity
        {
            return new MessageParameters(
                _messagesProvider.CreateEntityListMessage<TEntity>(actionMode, pageParameters.totalEntitiesCount),
                _keyboardsProvider.CreateEntityListKeyboard(entities, actionMode, 1, (pageParameters.page, pageParameters.totalPagesCount), litterOwnerId));
        }

        public MessageParameters GetEntityFromLitterParameters(IDisplayableEntity entity, int litterId)
        {
            return new MessageParameters(
                _messagesProvider.CreateEntityFormMessage(entity, false),
                _keyboardsProvider.CreateEntityFromLitterKeyboard(entity, litterId),
                [.. entity.Photos.Take(1).Select(p => new InputMediaPhoto(p.TelegramPhoto?.FileId ?? string.Empty))]);
        }

        public MessageParameters GetColorPickerParameters(Cat cat, string buildedColor, string[] findedColorParts)
        {
            return new MessageParameters(
                _messagesProvider.CreateColorPickerMessage(cat, buildedColor),
                _keyboardsProvider.CreateColorPickerKeyboard(cat, buildedColor, findedColorParts));
        }

        public MessageParameters GetKittenClassParameters(Kitten kitten)
        {
            return new MessageParameters(
                _messagesProvider.CreateKittenClassSelectionMenuMessage(kitten),
                _keyboardsProvider.CreateKittenClassSelectionKeyboard(kitten));
        }

        public MessageParameters GetKittenStatusParameters(Kitten kitten)
        {
            return new MessageParameters(
                _messagesProvider.CreateKittenStatusSelectionMenuMessage(kitten),
                _keyboardsProvider.CreateKittenStatusSelectionKeyboard(kitten));
        }

        public MessageParameters GetNewBookingRequestParameters(BookingRequest bookingRequest)
        {
            return new MessageParameters(
                _messagesProvider.CreateNewBookingRequestMessage(bookingRequest),
                _keyboardsProvider.CreateBookingRequestTakeCuratorshipKeyboard(bookingRequest));
        }

        public MessageParameters GetBookingProcessingParameters(BookingRequest bookingRequest, User curator)
        {
            return new MessageParameters(
                _messagesProvider.CreateBookingProcessingMessage(bookingRequest, curator),
                _keyboardsProvider.CreateBookingRequestCloseKeyboard(bookingRequest));
        }

        public MessageParameters GetBookingClosedParameters(string previousMessageText, int kittenId)
        {
            return new MessageParameters(
                previousMessageText + _messagesProvider.CreateBookingClosedMessage(),
                _keyboardsProvider.CreateBookingChangeKittenStatusKeyboard(kittenId));
        }
    }
}
