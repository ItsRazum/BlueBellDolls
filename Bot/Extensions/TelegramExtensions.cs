using BlueBellDolls.Bot.Adapters;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Bot.Extensions
{
    public static class TelegramExtensions
    {
        public static MessageAdapter ToAdaper(this Message message, IEnumerable<PhotoAdapter>? photos = null)
        {
            return new MessageAdapter(message, photos);
        }

        public static CallbackQueryAdapter ToAdaper(this CallbackQuery callback, string? callbackData = null)
        {
            if (callbackData != null)
                callback.Data = callbackData;

            return new CallbackQueryAdapter(callback);
        }

        public static PhotoAdapter ToAdapter(this PhotoSize photoSize, int messageId)
        {
            return new PhotoAdapter(photoSize, messageId);
        }

        public static PhotoAdapter? GetPhotoAdapter(this Message message)
        {
            if (message.Photo != null)
                return new PhotoAdapter(message.Photo.Last(), message.Id);

            return null;
        }

        public static IEnumerable<InlineKeyboardButton[]> ToEnumerable(this InlineKeyboardMarkup inlineKeyboardMarkup)
        {
            foreach (var inlineKeyboardButtonRow in inlineKeyboardMarkup.InlineKeyboard.Select(r => r.ToArray()))
                yield return inlineKeyboardButtonRow;
        }
    }
}
