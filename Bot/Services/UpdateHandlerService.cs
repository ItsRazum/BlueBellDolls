using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Extensions;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BlueBellDolls.Bot.Services
{
    internal class UpdateHandlerService : IUpdateHandlerService, IHostedService
    {

        #region Fields

        private readonly ILogger<UpdateHandlerService> _logger;
        private readonly CallbackDataSettings _callbackDataSettings;
        private readonly IBotService _botService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ConcurrentDictionary<string, List<PhotoAdapter>> _photoUploaders;
        private readonly long[] _authorizedUsers;

        #endregion

        #region Constructor

        public UpdateHandlerService(
            ILogger<UpdateHandlerService> logger,
            IOptions<BotSettings> options,
            IBotService botService,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _callbackDataSettings = options.Value.CallbackDataSettings;
            _authorizedUsers = options.Value.AuthorizedUsers;
            _botService = botService;
            _serviceScopeFactory = serviceScopeFactory;

            _photoUploaders = [];
        }

        #endregion

        #region IUpdateHandlerService implementation

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            _logger.LogDebug(JsonConvert.SerializeObject(update.Message));
            Chat? chat = null;
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        if (update.Message is not null)
                        {
                            var message = update.Message;
                            chat = message.Chat;

                            if (!_authorizedUsers.Contains(message.From?.Id ?? -1))
                                return;

                            if (message.ReplyToMessage is not null)
                                await HandleReplyToMessageAsync(update.Message, cancellationToken);

                            else
                                await HandleCommandAsync(message, message.Text != null && message.Text.Contains(' '), cancellationToken);
                        }
                        break;

                    case UpdateType.CallbackQuery:
                        if (!_authorizedUsers.Contains(update.CallbackQuery?.From.Id ?? -1))
                            return;

                        if (update.CallbackQuery is not null)
                        {
                            chat = update.CallbackQuery.Message?.Chat;
                            await HandleCallbackAsync(update.CallbackQuery, update.CallbackQuery.Data!.Contains(_callbackDataSettings.ArgsSeparator), cancellationToken);
                        }

                        break;

                    default:
                        _logger.LogWarning("Необработанный тип обновления: {UpdateType}", update.Type);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла непредвиденная ошибка!");
                if (chat != null)
                    await _botService.SendMessageAsync(chat, "Произошла необработанная ошибка, пожалуйста, свяжитесь с разработчиком бота (@NtRazum)", token: cancellationToken);
            }
        }

        public async Task HandleCommandAsync(Message m, bool containsArgs, CancellationToken token = default)
        {
            if (m.Text != null)
            {
                _logger.LogInformation("=== {message} ===" +
                        $"\nId отправителя: {m.From?.Id}" +
                        $"\nUsername отправителя: {"@" + m.From?.Username}" +
                        $"\nИмя отправителя: {m.From?.FirstName}" +
                        $"\n" +
                        $"\nТекст сообщения: {m.Text}" +
                        $"\n" +
                        $"\nЧат: {m.Chat.Type}" +
                        $"\nId чата: {m.Chat.Id}" +
                        $"\n", "Входящее сообщение");

                if (containsArgs)
                    m.Text = Regex.Replace(m.Text, @"\s{2,}", " ").Trim();

                var commandData = (containsArgs ? m.Text.Split(' ').First() : m.Text).ToLower();

                using var scope = _serviceScopeFactory.CreateScope();
                var messageHandlers = scope.ServiceProvider
                    .GetRequiredService<IEnumerable<CommandHandler>>();

                var commandFunc = messageHandlers
                    .SelectMany(h => h.Handlers)
                    .FirstOrDefault(kvp => kvp.Key == commandData)
                    .Value;

                if (commandFunc != null)
                {
                    await commandFunc(m.ToAdaper(), token);
                }
            }
        }

        public async Task HandleCallbackAsync(CallbackQuery c, bool containsArgs, CancellationToken token = default)
        {
            _logger.LogInformation("=== {callback} ===" +
                        $"\nCallbackId: {c.Id}" +
                        $"\nCallbackData: {c.Data}" +
                        $"\n" +
                        $"\nId отправителя: {c.From.Id}" +
                        $"\nUsername отправителя: {"@" + c.From.Username}" +
                        $"\nИмя отправителя: {c.From.FirstName}" +
                        $"\n" +
                        $"\nЧат: {c.Message?.Chat.Type}" +
                        $"\nId чата: {c.Message?.Chat.Id}" +
                        $"\n", "Входящий Callback");

            foreach (var callback in c.Data!.Split(_callbackDataSettings.MultipleCallbackSeparator))
            {
                if (callback == "...") return;

                var commandData = containsArgs ? callback.Split(_callbackDataSettings.ArgsSeparator).First() : callback;

                using var scope = _serviceScopeFactory.CreateScope();
                var callbackHandlers = scope.ServiceProvider
                    .GetRequiredService<IEnumerable<CallbackHandler>>();

                var commandFunc = callbackHandlers
                    .SelectMany(h => h.Handlers)
                    .FirstOrDefault(kvp => kvp.Key == commandData)
                    .Value;

                if (commandFunc != null)
                {
                    await commandFunc(c.ToAdaper(callback), token);
                }
            }
        }

        public async Task HandleReplyToMessageAsync(Message m, CancellationToken token = default)
        {
            _logger.LogInformation("=== {message} ===" +
                $"\nId отправителя: {m.From?.Id}" +
                $"\nUsername отправителя: {"@" + m.From?.Username}" +
                $"\nИмя отправителя: {m.From?.FirstName}" +
                $"\n" +
                $"\nТекст сообщения: {m.Text}" +
                $"\n" +
                $"\nЧат: {m.Chat.Type}" +
                $"\nId чата: {m.Chat.Id}" +
                $"\n", "Входящее сообщение");

            if (m.Photo != null)
            {
                var mediaGroupId = m.MediaGroupId;

                if (mediaGroupId != null)
                {
                    if (!_photoUploaders.TryGetValue(mediaGroupId, out var value))
                    {
                        _photoUploaders.TryAdd(mediaGroupId, []);
                        value = _photoUploaders[mediaGroupId];

                        var timer = new System.Timers.Timer(100);
                        timer.Elapsed += async (s, e) =>
                        {
                            timer.Stop();
                            timer.Dispose();

                            if (m.Caption == null)
                            {
                                _photoUploaders.Remove(mediaGroupId, out _);
                                return;
                            }

                            m.Text = m.Caption;
                            
                            await tryHandleCommand(m, value, token);
                            _photoUploaders.Remove(mediaGroupId, out _);
                        };
                        timer.Start();
                    }

                    value.Add(m.GetPhotoAdapter()!);
                    return;
                }

                await tryHandleCommand(m, [m.GetPhotoAdapter()!], token);
            }
            

            await tryHandleCommand(m, token: token);

            async Task tryHandleCommand(Message m, IEnumerable<PhotoAdapter>? photos = null, CancellationToken token = default)
            {
                var command = (m.Text ?? m.Caption ?? string.Empty).ToLower();
                var replyData = (m.ReplyToMessage!.Text ?? m.ReplyToMessage.Caption ?? string.Empty).ToLower();


                using var scope = _serviceScopeFactory.CreateScope();
                var messageHandlers = scope.ServiceProvider
                    .GetRequiredService<IEnumerable<CommandHandler>>();

                var commandFunc = messageHandlers
                    .SelectMany(h => h.Handlers)
                    .FirstOrDefault(kvp => kvp.Key == command || kvp.Key == $"update_entity_by_reply_{replyData.Split(' ').First()}")
                    .Value;

                if (commandFunc != null)
                {
                    await commandFunc(m.ToAdaper(photos), token);
                }
            }
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        #endregion

        #region IHostedService implementation

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _botService.StartReceiving(this, new ReceiverOptions
            {
                AllowedUpdates =
                    [
                        UpdateType.Message,
                        UpdateType.CallbackQuery
                    ]
            }, 
            cancellationToken);
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
