using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Bot.Services
{
    internal class BotService : IBotService, IHostedService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<BotService> _logger;
        private readonly BotSettings _botSettings;
        private readonly IServiceProvider _serviceProvider;

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

        public async Task AnswerCallbackQueryAsync(string callbackQueryId, string text, bool showAlert = true, CancellationToken token = default)
        {
            await _botClient.AnswerCallbackQuery(callbackQueryId, text, showAlert, cancellationToken: token);
        }

        public async Task EditMessageAsync(Chat chat, int messageId, string messageText, InlineKeyboardMarkup? inlineMarkup = null, CancellationToken token = default)
        {
            await _botClient.EditMessageText(chat, messageId, messageText, replyMarkup: inlineMarkup, cancellationToken: token);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var rootLocation = _serviceProvider.GetService<IRootLocation>();

            if (rootLocation == null)
            {
                ArgumentNullException ex = new(nameof(rootLocation));
                _logger.LogCritical(ex, "RootLocation is not registered in container.");
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
                        UpdateType.InlineQuery
                    ]
                },
                default
                );

            _logger.LogInformation("Бот запущен.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Бот остановлен.");
            return Task.CompletedTask;
        }
    }
}
