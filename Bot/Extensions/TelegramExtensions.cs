using BlueBellDolls.Bot.Adapters;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Bot.Extensions
{
    public static class TelegramExtensions
    {
        public static MessageAdapter ToAdaper(this Message message)
        {
            return new MessageAdapter(message);
        }

        public static CallbackQueryAdapter ToAdaper(this CallbackQuery callback, string? callbackData = null)
        {
            if (callbackData != null)
                callback.Data = callbackData;

            return new CallbackQueryAdapter(callback);
        }

        public static IEnumerable<InlineKeyboardButton[]> ToEnumerable(this InlineKeyboardMarkup inlineKeyboardMarkup)
        {
            foreach (var inlineKeyboardButtonRow in inlineKeyboardMarkup.InlineKeyboard.Select(r => r.ToArray()))
                yield return inlineKeyboardButtonRow;
        }
    }
}
