using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Common.Types
{
    public class MessageParameters(string text, ReplyMarkup? replyMarkup = null, IEnumerable<InputMediaPhoto>? inputFiles = null)
    {
        public string Text { get; } = text;
        public ReplyMarkup? ReplyMarkup { get; } = replyMarkup;
        public InputMediaPhoto[]? InputFiles { get; } = inputFiles?.ToArray();
    }
}
