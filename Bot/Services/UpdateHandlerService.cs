using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Extensions;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BlueBellDolls.Bot.Services
{
    internal class UpdateHandlerService : IUpdateHandlerService, IUpdateHandler
    {
        private readonly ILogger<UpdateHandlerService> _logger;
        private readonly ConcurrentDictionary<long, string> _photoUploaders;

        private Dictionary<string, Func<MessageAdapter, CancellationToken, Task>> TextCommands { get; set; }
        private Dictionary<string, Func<CallbackQueryAdapter, CancellationToken, Task>> CallbackCommands { get; set; }

        public UpdateHandlerService(
            ILogger<UpdateHandlerService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _photoUploaders = [];

            TextCommands = [];
            CallbackCommands = [];

            var messageHandlers = serviceProvider.GetServices<CommandHandler<MessageAdapter>>();
            var callbackHandlers = serviceProvider.GetServices<CommandHandler<CallbackQueryAdapter>>();

            foreach (var command in messageHandlers)
                foreach (var commandHandler in command.Handlers)
                    TextCommands.Add(commandHandler.Key, commandHandler.Value);

            foreach (var callback in callbackHandlers)
                foreach (var callbackHandler in callback.Handlers)
                    CallbackCommands.Add(callbackHandler.Key, callbackHandler.Value);
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation(JsonConvert.SerializeObject(update));
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message is not null)
                    {
                        if (update.Message.ReplyToMessage is not null && update.Message.ReplyToMessage.Text != null)
                            await HandleReplyToMessageAsync(update.Message, cancellationToken);
                        else
                            await HandleCommandAsync(update.Message, update.Message.Text != null && update.Message.Text.Contains(' '), cancellationToken);
                    }
                    break;

                case UpdateType.CallbackQuery:
                    if (update.CallbackQuery is not null)
                        await HandleCallbackAsync(update.CallbackQuery, update.CallbackQuery.Data!.Contains('-'), cancellationToken);
                    break;

                default:
                    _logger.LogWarning("Необработанный тип обновления: {UpdateType}", update.Type);
                    break;
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

                if (TextCommands.TryGetValue(commandData, out var command))
                    await command(m.ToAdaper(), token);
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

            foreach(var callback in c.Data!.Split('\n'))
            {
                var commandData = containsArgs ? callback.Split('-').First() : callback;

                if (CallbackCommands.TryGetValue(commandData, out var command))
                    await command(c.ToAdaper(callback), token);
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

            if (TextCommands.TryGetValue($"updateEntityByReply-{m.ReplyToMessage!.Text!.Split(' ').First()}", out var commandHandler)
                || TextCommands.TryGetValue(m.Text ?? string.Empty, out commandHandler))
                await commandHandler(m.ToAdaper(), token);
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
