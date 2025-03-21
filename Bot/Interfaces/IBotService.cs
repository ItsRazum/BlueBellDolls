using BlueBellDolls.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IBotService
    {

        #region Methods

        /// <summary>
        /// Запускает отслеживание обновлений.
        /// </summary>
        void StartReceiving(IUpdateHandlerService updateHandler, ReceiverOptions receiverOptions, CancellationToken token);

        /// <summary>
        /// Формирует и отправляет сообщение в зависимости от переданных параметров.
        /// Переданный <paramref name="inputFiles"/> отправит медиагруппу, если фотографий в коллекции больше одной
        /// </summary>
        /// <param name="inputFiles"></param>
        /// <returns></returns>
        Task<Message[]> SendMessageAsync(Chat chat, string messageText, IReplyMarkup? replyMarkup = null, InputMediaPhoto[]? inputFiles = null, CancellationToken token = default);

        /// <summary>
        /// Формирует и отправляет сообщение из <paramref name="messageParameters"/>.
        /// </summary>
        /// <param name="messageParameters"></param>
        /// <returns></returns>
        async Task<Message[]> SendMessageAsync(Chat chat, MessageParameters messageParameters, CancellationToken token = default)
            => await SendMessageAsync(chat, messageParameters.Text, messageParameters.InlineKeyboard, messageParameters.InputFiles, token);

        /// <summary>
        /// Отправляет ответ на CallbackQuery.
        /// </summary>
        Task AnswerCallbackQueryAsync(string callbackQueryId, string text, bool showAlert = true, CancellationToken token = default);

        /// <summary>
        /// Редактирует текст и клавиатуру сообщения.
        /// </summary>
        /// <returns>true, если сообщение успешно отредактировано, false, если сообщение не удалось отредактировать.</returns>
        Task<bool> EditMessageAsync(Chat chat, int messageId, string messageText, InlineKeyboardMarkup? inlineMarkup = null, InputMediaPhoto? inputMediaPhoto = null, CancellationToken token = default);

        /// <summary>
        /// Редактирует текст и клавиатуру сообщения через данные из <paramref name="messageParameters"/>.
        /// </summary>
        /// <param name="messageParameters"></param>
        /// <returns>true, если сообщение успешно отредактировано, false, если сообщение не удалось отредактировать.</returns>
        async Task<bool> EditMessageAsync(Chat chat, int messageId, MessageParameters messageParameters, CancellationToken token = default)
            => await EditMessageAsync(
                chat,
                messageId, 
                messageParameters.Text,
                messageParameters.InlineKeyboard,
                messageParameters.InputFiles?.First(),
                token);

        /// <summary>
        /// Пытается отредактировать сообщение, упираясь в ограничения Telegram API. Если отредактировать сообщение не удаётся, то оно удаляется, и отправляется новое сообщение с теми-же параметрами 
        /// </summary>
        Task EditOrSendNewMessageAsync(Chat chat, int messageId, string messageText, InlineKeyboardMarkup? inlineMarkup = null, InputMediaPhoto[]? inputFiles = null, CancellationToken token = default);

        /// <summary>
        /// Пытается отредактировать сообщение, упираясь в ограничения Telegram API. Если отредактировать сообщение не удаётся, то оно удаляется, и отправляется новое сообщение с теми-же параметрами 
        /// </summary>
        async Task EditOrSendNewMessageAsync(Chat chat, int messageId, MessageParameters messageParameters, CancellationToken token = default)
            => await EditOrSendNewMessageAsync(
                chat,
                messageId,
                messageParameters.Text,
                messageParameters.InlineKeyboard,
                messageParameters.InputFiles,
                token);
        
        /// <summary>
        /// Удаляет сообщение.
        /// </summary>
        /// <returns>true, если сообщение успешно удалилось, false, если сообщение не удалилось.</returns>
        Task<bool> DeleteMessageAsync(Chat chat, int messageId, CancellationToken token = default);

        /// <summary>
        /// Удаляет несколько сообщений одновременно.
        /// </summary>
        Task<bool> DeleteMessagesAsync(Chat chat, IEnumerable<int> messageIds, CancellationToken token = default);

        /// <summary>
        /// Получает информацию о файле, хранящемся на сервере Telegram, по его fileId
        /// </summary>
        Task<TGFile> GetFileAsync(string fileId, CancellationToken token = default);

        #endregion

    }
}
