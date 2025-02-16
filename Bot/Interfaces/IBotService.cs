using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IBotService
    {

        #region Methods

        Task SendMessageAsync(Chat chat, string messageText, IReplyMarkup? replyMarkup = null, IEnumerable<InputMediaPhoto>? inputFiles = null, CancellationToken token = default);

        Task SendMessageAsync(Chat chat, MessageParameters messageParameters, CancellationToken token = default);

        Task AnswerCallbackQueryAsync(string callbackQueryId, string text, bool showAlert = true, CancellationToken token = default);

        Task<bool> EditMessageAsync(Chat chat, int messageId, string messageText, InlineKeyboardMarkup? inlineMarkup = null, InputMediaPhoto? inputMediaPhoto = null, CancellationToken token = default);

        Task<bool> EditMessageAsync(Chat chat, int messageId, MessageParameters messageParameters, CancellationToken token = default);

        Task EditOrSendNewMessageAsync(Chat chat, int messageId, string messageText, InlineKeyboardMarkup? inlineMarkup = null, IEnumerable<InputMediaPhoto>? inputFiles = null, CancellationToken token = default);

        Task EditOrSendNewMessageAsync(Chat chat, int messageId, MessageParameters messageParameters, CancellationToken token = default);

        Task<bool> DeleteMessageAsync(Chat chat, int messageId, CancellationToken token = default);

        #endregion

    }
}
