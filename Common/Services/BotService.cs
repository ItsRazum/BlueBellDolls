using BlueBellDolls.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Common.Services
{
    public class BotService(
        ILogger<BotService> logger,
        ITelegramBotClient botClient) : IBotService
    {

        #region Fields

        private readonly ITelegramBotClient _botClient = botClient;
        private readonly ILogger<BotService> _logger = logger;

        #endregion

        #region IBotService implementation

        public void StartReceiving(
            IUpdateHandlerService updateHandler,
            ReceiverOptions receiverOptions,
            CancellationToken token)
        {
            _botClient.StartReceiving(updateHandler, receiverOptions, cancellationToken: token);
        }

        public async Task<Message[]> SendMessageAsync(Chat chat, string messageText, IReplyMarkup? replyMarkup = null, InputMediaPhoto[]? inputFiles = null, CancellationToken token = default)
        {
            if (inputFiles?.Length > 0)
            {
                inputFiles.First().Caption = messageText;

                if (inputFiles.Length > 1)
                {
                    return await _botClient.SendMediaGroup(chat, inputFiles, cancellationToken: token);
                }
                else
                    return [await _botClient.SendPhoto(chat, inputFiles.First().Media, messageText, replyMarkup: replyMarkup, cancellationToken: token)];
            }
            else
                return [await _botClient.SendMessage(chat, messageText, replyMarkup: replyMarkup, parseMode: ParseMode.Html, cancellationToken: token)];
        }

        public async Task AnswerCallbackQueryAsync(string callbackQueryId, string text, bool showAlert = true, CancellationToken token = default)
        {
            await _botClient.AnswerCallbackQuery(callbackQueryId, text, showAlert, cancellationToken: token);
        }

        public async Task<bool> EditMessageAsync(Chat chat, int messageId, string messageText, InlineKeyboardMarkup? inlineMarkup = null, InputMediaPhoto? inputMediaPhoto = null, CancellationToken token = default)
        {
            try
            {
                if (inputMediaPhoto != null)
                {
                    inputMediaPhoto.Caption = messageText;
                    await _botClient.EditMessageMedia(chat, messageId, inputMediaPhoto, inlineMarkup, cancellationToken: token);
                }
                else
                    await _botClient.EditMessageText(chat, messageId, messageText, replyMarkup: inlineMarkup, parseMode: ParseMode.Html, cancellationToken: token);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось изменить сообщение");
                return false;
            }
        }

        public async Task<bool> EditInlineKeyboardAsync(Chat chat, int messageId, InlineKeyboardMarkup inlineKeyboard, CancellationToken token = default)
        {
            try
            {
                await _botClient.EditMessageReplyMarkup(chat, messageId, inlineKeyboard, cancellationToken: token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось изменить клавиатуру для сообщения");
                return false;
            }
        }

        public async Task EditOrSendNewMessageAsync(
            Chat chat,
            int messageId,
            string messageText,
            InlineKeyboardMarkup? inlineMarkup = null,
            InputMediaPhoto[]? inputFiles = null,
            CancellationToken token = default)
        {
            var hasFiles = inputFiles?.Length > 0;
            var editSuccess = false;

            if (!hasFiles)
                editSuccess = await EditMessageAsync(chat, messageId, messageText, inlineMarkup, token: token);

            else if (inputFiles!.Length == 1)
                editSuccess = await EditMessageAsync(chat, messageId, messageText, inlineMarkup, inputFiles?.First(), token);

            if (editSuccess) return;

            if (!await DeleteMessageAsync(chat, messageId, token))
                await EditMessageAsync(chat, messageId, "Содержимое стёрто", token: token);

            await SendMessageAsync(chat, messageText, inlineMarkup, hasFiles ? inputFiles : null, token);
        }

        public async Task<bool> DeleteMessageAsync(Chat chat, int messageId, CancellationToken token = default)
        {
            try
            {
                await _botClient.DeleteMessage(chat, messageId, token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{BotService}.{DeleteMessageAsync}: Вызвано исключение", nameof(BotService), nameof(DeleteMessageAsync));
                return false;
            }
        }

        public async Task<bool> DeleteMessagesAsync(Chat chat, IEnumerable<int> messageIds, CancellationToken token = default)
        {
            try
            {
                await _botClient.DeleteMessages(chat, messageIds, token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Не удалось удалить сообщения");
                return false;
            }
        }

        public async Task<TGFile> GetFileAsync(string fileId, CancellationToken token = default)
        {
            return await _botClient.GetFile(fileId, token);
        }

        #endregion

    }
}
