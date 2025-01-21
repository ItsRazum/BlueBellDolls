using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Bot.Interfaces
{
    internal interface IBotService
    {
        Task SendMessageAsync(Chat chat, string messageText, IReplyMarkup? replyMarkup = null, IEnumerable<InputMediaPhoto>? inputFiles = null, CancellationToken token = default);
    }
}
