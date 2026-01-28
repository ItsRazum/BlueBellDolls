using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Interfaces.Providers
{
    public interface IMessageParametersProvider
    {
        MessageParameters GetEntityFormParameters(IDisplayableEntity entity);

        MessageParameters GetEntityPhotosParameters(
            IDisplayableEntity entity, 
            PhotosType photosType, 
            int[] selectedPhotoIndexes, 
            int[] photoMessageIds);

        MessageParameters GetDeleteEntityConfirmationParameters(
            IDisplayableEntity entity, 
            string callback, 
            string onDeletionCanceledCallback);

        MessageParameters GetDeleteEntityPhotosConfirmationParameters(
            IDisplayableEntity entity, 
            string callback, 
            int[] selectedPhotoIndexes, 
            int[] sendedPhotoMessageIds,
            string onDeletionCanceledCallback);

        MessageParameters GetEntityListParameters<TEntity>(
            IEnumerable<TEntity> entities, 
            ListUnitActionMode actionMode, 
            (int page, int totalPagesCount, int totalEntitiesCount) pageParameters,
            int? litterOwnderId = null)
            where TEntity : class, IDisplayableEntity;

        MessageParameters GetEntityFromLitterParameters(IDisplayableEntity entity, int litterId);

        MessageParameters GetColorPickerParameters(Cat cat, string buildedColor, string[] findedColorParts);

        MessageParameters GetStartParameters();

        MessageParameters GetKittenClassParameters(Kitten kitten);

        MessageParameters GetKittenStatusParameters(Kitten kitten);

        MessageParameters GetNewBookingRequestParameters(BookingRequest bookingRequest);

        MessageParameters GetBookingProcessingParameters(BookingRequest bookingRequest, User curator);

        MessageParameters GetBookingClosedParameters(string previousMessageText, int kittenId);
    }
}