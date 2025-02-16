using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Bot.Types
{
    public class MessageParameters(string text, InlineKeyboardMarkup? inlineKeyboard = null, IEnumerable<InputMediaPhoto>? inputFiles = null)
    {
        public string Text { get; } = text;
        public InlineKeyboardMarkup? InlineKeyboard { get; } = inlineKeyboard;
        public IEnumerable<InputMediaPhoto>? InputFiles { get; } = inputFiles;
    }
}
