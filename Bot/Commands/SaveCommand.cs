using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types;

namespace BlueBellDolls.Bot.Commands
{
    public class SaveCommand : CommandHandler
    {
        private readonly IManagementService _managementService;
        private readonly IMessagesProvider _messagesProvider;

        public SaveCommand(
            IBotService botService, 
            IManagementService managementService,
            IMessagesProvider messagesProvider)
            : base(botService)
        {
            _managementService = managementService;
            _messagesProvider = messagesProvider;

            AddCommandHandler("/save", HandleCommandAsync);
        }

        private async Task HandleCommandAsync(MessageAdapter m, CancellationToken token)
        {
            var result = await _managementService.ActivateEntitiesAsync(token);

            if (result.Success)
                await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreateSavingSuccessMessage(result.Result!.Value), token: token);

            else
                await BotService.SendMessageAsync(m.Chat, result.ErrorText!, token: token);
        }
    }
}
