using BlueBellDolls.Common.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Common.Providers
{
    public class CommonKeyboardsProvider : ICommonKeyboardsProvider
    {
        //TODO: Синхронизировать сборку клавиатур с клиентом бота-менеджерки, возможно какой-то отдельный проект делать придётся
        public InlineKeyboardMarkup CreateProcessBookingKeyboard(int bookingId)
            => InlineKeyboardButton.WithCallbackData("Взять в работу", $"processBookingRequest-{bookingId}");
    }
}
