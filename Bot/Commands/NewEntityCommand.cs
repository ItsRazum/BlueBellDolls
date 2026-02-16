using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;

namespace BlueBellDolls.Bot.Commands
{
    public class NewEntityCommand : CommandHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IManagementServicesProvider _managementServicesProvider;
        private readonly IMessagesProvider _messagesProvider;

        public NewEntityCommand(
            IBotService botService,
            IMessageParametersProvider messageParametersProvider,
            IManagementServicesProvider managementServicesProvider,
            IMessagesProvider messagesProvider)
            : base(botService)
        {
            _messageParametersProvider = messageParametersProvider;
            _managementServicesProvider = managementServicesProvider;
            _messagesProvider = messagesProvider;

            AddCommandHandler("/newcat", HandleCommandAsync<ParentCat>);
            AddCommandHandler("/newlitter", HandleCommandAsync<Litter>);
        }

        private async Task HandleCommandAsync<TEntity>(MessageAdapter m, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var managementService = _managementServicesProvider.GetEntityManagementService<TEntity>();
            var result = await managementService.AddNewEntityAsync(token);

            if (result.Success)
                await BotService.SendMessageAsync(m.Chat, _messageParametersProvider.GetEntityFormParameters(result.Value!), token);
            else
                await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreateServerErrorMessage(result.Message), token: token);
        }
    }
}
