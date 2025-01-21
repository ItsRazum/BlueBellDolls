using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Extensions;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BlueBellDolls.Bot.Services
{
    internal class RootLocation : IRootLocation, IUpdateHandler
    {
        private readonly ILogger<RootLocation> _logger;
        private readonly IBotService _botService;
        private readonly IDatabaseService _dataBaseService;
        private readonly IMessagesFactory _messagesFactory;
        private readonly IEntityFormService _entityFormService;

        private Dictionary<string, Command<MessageAdapter>> TextCommands { get; }
        private Dictionary<string, Command<CallbackQueryAdapter>> CallbackCommands { get; }


        public RootLocation(
            IBotService botService,
            ILogger<RootLocation> logger,
            IDatabaseService databaseService,
            IMessagesFactory messagesFactory,
            IEntityFormService entityFormService)
        {
            _logger = logger;
            _botService = botService;
            _dataBaseService = databaseService;
            _messagesFactory = messagesFactory;
            _entityFormService = entityFormService;

            TextCommands = new()
            {
                { "/start", new Command<MessageAdapter>(StartCommand) },
                { "/newcat", new Command<MessageAdapter>(NewCatCommand) }
            };
            CallbackCommands = [];
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message is not null)
                    {
                        if (update.Message.ReplyToMessage is not null)
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
                    await command.ExecuteAsync(m.ToAdaper(), token);
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

            var commandData = containsArgs ? c.Data!.Split('-').First() : c.Data!;

            if (CallbackCommands.TryGetValue(commandData, out var command))
                await command.ExecuteAsync(c.ToAdaper(), token);
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

            if (m.ReplyToMessage?.Text != null)
            {
                var args = m.ReplyToMessage.Text.Split('\n');

                if (IsValidMessageFormat(args.First(), out var modelName, out var modelId))
                {
                    var properties = m.Text?.Split('\n')
                        .Select(line => line.Split(": ", 2))
                        .Where(parts => parts.Length == 2)
                        .ToDictionary(parts => parts[0], parts => parts[1]);

                    if (properties == null || properties.Count == 0)
                    {
                        return;
                    }

                    await UpdateEntityAsync(modelName, modelId, properties, token);
                }

                static bool IsValidMessageFormat(string message, out string modelName, out int modelId)
                {
                    modelName = string.Empty;
                    modelId = 0;

                    var parts = message.Split(' ');
                    if (parts.Length < 2)
                        return false;

                    modelName = parts[0];
                    if (!int.TryParse(parts[1], out modelId))
                        return false;

                    return true;
                }
            }
        }

        private async Task UpdateEntityAsync(
            string modelName,
            int modelId,
            Dictionary<string, string> properties,
            CancellationToken token)
        {
            await _dataBaseService.ExecuteDbOperationAsync(async (unit, ct) =>
            {
                IEntity? model = modelName switch
                {
                    "ParentCat" => await unit.GetRepository<ParentCat>().GetByIdAsync(modelId, ct),
                    "Kitten" => await unit.GetRepository<Kitten>().GetByIdAsync(modelId, ct),
                    "Litter" => await unit.GetRepository<Litter>().GetByIdAsync(modelId, ct),
                    _ => throw new FormatException(modelName)
                };
                if (model != null)
                    foreach (var (propertyName, value) in properties)
                    {
                        _entityFormService.UpdateProperty(model, propertyName, value);
                    }

                await unit.SaveChangesAsync(ct);
            }, token);
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {

        }


        private async Task StartCommand(MessageAdapter m, CancellationToken token)
        {
            await _botService.SendMessageAsync(m.Chat, _messagesFactory.GetStartMessage(), token: token);
        }

        private async Task NewCatCommand(MessageAdapter m, CancellationToken token)
        {
            var newCat = await _dataBaseService.GetDataAsync(async (unit, ct) =>
            {
                var result = new ParentCat()
                {
                    Name = "0",
                    BirthDay = DateOnly.FromDateTime(DateTime.Now),
                    IsMale = true,
                    Description = "0",
                    Photos = [],
                    Titles = [],
                    GeneticTestOne = string.Empty,
                    GeneticTestTwo = string.Empty
                };
                await unit.GetRepository<ParentCat>().AddAsync(result, ct);

                await unit.SaveChangesAsync(ct);

                return result;
            }, token);

            await _botService.SendMessageAsync(m.Chat, _messagesFactory.GetParentCatFormMessage(newCat), token: token);
        }
    }
}
