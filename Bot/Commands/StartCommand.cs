using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Commands
{
    public class StartCommand : CommandHandler
    {
        private readonly IMessagesProvider _messagesProvider;

        public StartCommand(
            IBotService botService,
            IMessagesProvider messagesProvider)
            : base(botService)
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
