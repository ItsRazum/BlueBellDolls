using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Bot.Interfaces.Providers
{
    public interface IKeyboardsProvider
    {
        InlineKeyboardMarkup CreateEntityListKeyboard<TEntity>(
            IEnumerable<TEntity> entities, 
            ListUnitActionMode actionMode = ListUnitActionMode.Edit,
            int chunkSize = 1,
            (int page, int totalPagesCount)? pageParameters = null, 
            int? litterOwnerId = null) 
            where TEntity : class, IDisplayableEntity;

        InlineKeyboardMarkup CreateEntityOptionsKeyboard(IDisplayableEntity entity);

        InlineKeyboardMarkup CreateEntityPhotosKeyboard(IDisplayableEntity entity, PhotosType photosUploadMode, int[] photoMessageIds, int[]? selectedPhotosIndexes = null);

        InlineKeyboardMarkup CreateYesNoKeyboard(string callback, IDisplayableEntity entity, string onDeletionCanceledCallback);

        InlineKeyboardMarkup CreateEntityFromLitterKeyboard(IDisplayableEntity entity, int litterId);

        InlineKeyboardMarkup CreateColorPickerKeyboard(Cat entity, string buildedColor, string[] findedColorParts);

        ReplyKeyboardMarkup CreateStartKeyboard();

        InlineKeyboardMarkup CreateKittenClassSelectionKeyboard(Kitten kitten);

        InlineKeyboardMarkup CreateKittenStatusSelectionKeyboard(Kitten kitten);

        InlineKeyboardMarkup CreateBookingRequestTakeCuratorshipKeyboard(BookingRequest bookingRequest);

        InlineKeyboardMarkup CreateBookingRequestCloseKeyboard(BookingRequest bookingRequest);

        InlineKeyboardMarkup CreateBookingChangeKittenStatusKeyboard(int kittenId);
    }
}
