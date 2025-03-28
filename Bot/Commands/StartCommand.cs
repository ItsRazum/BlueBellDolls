using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Commands
{
    public class StartCommand : CommandHandler
    {
        private readonly IMessagesProvider _messagesProvider;

        public StartCommand(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            IMessagesProvider messagesProvider)
            : base(botService, botSettings)
        {
            _messagesProvider = messagesProvider;

            AddCommandHandler("/start", HandleCommandAsync);
        }

        public Func<MessageAdapter, CancellationToken, Task> Handler => HandleCommandAsync;

        private async Task HandleCommandAsync(MessageAdapter m, CancellationToken token)
        {
            await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreateStartMessage(), token: token);
        }
    }
}
