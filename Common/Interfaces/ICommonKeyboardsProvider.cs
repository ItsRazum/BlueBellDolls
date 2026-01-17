using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Common.Interfaces
{
    public interface ICommonKeyboardsProvider
    {
        InlineKeyboardMarkup CreateProcessBookingKeyboard(int bookingId);
    }
}