using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Bot.Commands
{
    public class StartCommand : CommandHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;

        public StartCommand(
            IBotService botService,
            IMessageParametersProvider messageParametersProvider)
            : base(botService)
        {
            _messageParametersProvider = messageParametersProvider;

            AddCommandHandler("/start", HandleCommandAsync);
            AddCommandHandler("/hide", HideCommandAsync);
        }

        private async Task HideCommandAsync(MessageAdapter m, CancellationToken token)
        {
            await BotService.SendMessageAsync(m.Chat, new MessageParameters(":)", new ReplyKeyboardRemove()), token);
        }

        public Func<MessageAdapter, CancellationToken, Task> Handler => HandleCommandAsync;

        private async Task HandleCommandAsync(MessageAdapter m, CancellationToken token)
        {
            await BotService.SendMessageAsync(m.Chat, _messageParametersProvider.GetStartParameters(), token: token);
        }
    }
}
