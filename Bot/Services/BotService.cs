using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Bot.Services
{
    public class BotService : IBotService, IHostedService
    {

        #region Fields

        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<BotService> _logger;
        private readonly BotSettings _botSettings;
        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Constructor

        public BotService(
            ILogger<BotService> logger,
            IOptions<BotSettings> botSettings,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _botSettings = botSettings.Value;
            _botClient = new TelegramBotClient(_botSettings.Token);
            _serviceProvider = serviceProvider;
        }

        #endregion

        #region IBotService implementation

        public async Task SendMessageAsync(Chat chat, string messageText, IReplyMarkup? replyMarkup = null, IEnumerable<InputMediaPhoto>? inputFiles = null, CancellationToken token = default)
        {
            if (inputFiles?.Any() == true)
            {
                var mediaList = inputFiles.ToArray();

                if (mediaList.Length > 1)
                    await _botClient.SendMediaGroup(chat, inputFiles, cancellationToken: token);

                else
                    await _botClient.SendPhoto(chat, mediaList.First().Media, messageText, cancellationToken: token);

                return;
            }
            else
                await _botClient.SendMessage(chat, messageText, replyMarkup: replyMarkup, cancellationToken: token);
        }

        public async Task SendMessageAsync(Chat chat, MessageParameters messageParameters, CancellationToken token = default)
        {
            await SendMessageAsync(chat, messageParameters.Text, messageParameters.InlineKeyboard, messageParameters.InputFiles, token);
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
                    await _botClient.EditMessageMedia(chat, messageId, inputMediaPhoto, inlineMarkup, cancellationToken: token);
                else
                    await _botClient.EditMessageText(chat, messageId, messageText, replyMarkup: inlineMarkup, cancellationToken: token);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось изменить сообщение");
                return false;
            }
        }

        public async Task<bool> EditMessageAsync(Chat chat, int messageId, MessageParameters messageParameters, CancellationToken token = default)
        {
            return await EditMessageAsync(chat, messageId, messageParameters.Text, messageParameters.InlineKeyboard, messageParameters.InputFiles?.First(), token);
        }

        public async Task EditOrSendNewMessageAsync(
            Chat chat, 
            int messageId, 
            string messageText,
            InlineKeyboardMarkup? inlineMarkup = null, 
            IEnumerable<InputMediaPhoto>? inputFiles = null,
            CancellationToken token = default)
        {
            var hasFiles = inputFiles?.Any() == true;
            var editSuccess = false;

            if (!hasFiles)
                editSuccess = await EditMessageAsync(chat, messageId, messageText, inlineMarkup, token: token);

            else if (inputFiles!.Count() == 1)
                editSuccess = await EditMessageAsync(chat, messageId, messageText, inlineMarkup, inputFiles?.First(), token);

            if (editSuccess) return;

            if (!await DeleteMessageAsync(chat, messageId, token))
                await EditMessageAsync(chat, messageId, "Содержимое стёрто", token: token);

            await SendMessageAsync(chat, messageText, inlineMarkup, hasFiles ? inputFiles : null, token);
        }

        public async Task EditOrSendNewMessageAsync(Chat chat, int messageId, MessageParameters messageParameters, CancellationToken token = default)
        {
            await EditOrSendNewMessageAsync(chat, messageId, messageParameters.Text, messageParameters.InlineKeyboard, messageParameters.InputFiles, token);
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

        #endregion

        #region IHostedService implementation

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var rootLocation = _serviceProvider.GetService<IUpdateHandlerService>();

            if (rootLocation == null)
            {
                ArgumentNullException ex = new(nameof(rootLocation));
                _logger.LogCritical(ex, "Класс RootLocation не зарегистрирован в контейнере.");
                throw ex;
            }

            _botClient.StartReceiving(
                rootLocation.HandleUpdateAsync,
                rootLocation.HandleErrorAsync,
                new ReceiverOptions
                {
                    AllowedUpdates =
                    [
                        UpdateType.Message,
                        UpdateType.CallbackQuery
                    ]
                },
                default
                );

            _logger.LogInformation("Бот запущен.");
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Бот остановлен.");
            await Task.CompletedTask;
        }

        #endregion

    }
}
