using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Bot.Types
{
    public class MessageParameters(string text, IReplyMarkup? replyMarkup = null, IEnumerable<InputMediaPhoto>? inputFiles = null)
    {
        public string Text { get; } = text;
        public IReplyMarkup? ReplyMarkup { get; } = replyMarkup;
        public InputMediaPhoto[]? InputFiles { get; } = inputFiles?.ToArray();
    }
}
