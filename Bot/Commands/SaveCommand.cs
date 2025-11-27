using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types;

namespace BlueBellDolls.Bot.Commands
{
    public class SaveCommand : CommandHandler
    {
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementServicesFactory _managementServicesFactory;

        public SaveCommand(
            IBotService botService,
            IMessagesProvider messagesProvider, 
            IManagementServicesFactory managementServicesFactory)
            : base(botService)
        {
            _managementServicesFactory = managementServicesFactory;
            _messagesProvider = messagesProvider;

            AddCommandHandler("/save", HandleCommandAsync);
        }

        private async Task HandleCommandAsync(MessageAdapter m, CancellationToken token)
        {
            //var managementService = _managementServicesFactory.GetEntityManagementService();
            //var result = await _entityManagementService.ActivateEntitiesAsync(token);

            //if (result.Success)
            //    await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreateSavingSuccessMessage(result.Result!.Value), token: token);

            //else
            //    await BotService.SendMessageAsync(m.Chat, result.ErrorText!, token: token);
        }
    }
}
