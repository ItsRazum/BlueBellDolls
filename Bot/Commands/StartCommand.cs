using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;

namespace BlueBellDolls.Bot.Commands
{
    public class StartCommand : CommandHandler<MessageAdapter>
    {
        private readonly IMessagesProvider _messagesProvider;

        public StartCommand(
            IBotService botService,
            IMessagesProvider messagesProvider)
            : base(botService)
        {
            _messagesProvider = messagesProvider;

            Handlers.Add("/start", HandleCommandAsync);
        }

        public Func<MessageAdapter, CancellationToken, Task> Handler => HandleCommandAsync;

        private async Task HandleCommandAsync(MessageAdapter m, CancellationToken token)
        {
            await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreateStartMessage(), token: token);
        }
    }
}
