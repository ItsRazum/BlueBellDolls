using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;

namespace BlueBellDolls.Bot.Commands
{
    public class NewEntityCommand : CommandHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IManagementServicesFactory _managementServicesFactory;

        public NewEntityCommand(
            IBotService botService,
            IMessageParametersProvider messageParametersProvider,
            IManagementServicesFactory managementServicesFactory)
            : base(botService)
        {
            _messageParametersProvider = messageParametersProvider;
            _managementServicesFactory = managementServicesFactory;

            AddCommandHandler("/newcat", HandleCommandAsync<ParentCat>);
            AddCommandHandler("/newlitter", HandleCommandAsync<Litter>);
        }

        private async Task HandleCommandAsync<TEntity>(MessageAdapter m, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var managementService = _managementServicesFactory.GetEntityManagementService<TEntity>();
            var result = await managementService.AddNewEntityAsync(token);

            if (result.Result != null)
                await BotService.SendMessageAsync(m.Chat, _messageParametersProvider.GetEntityFormParameters(result.Result), token);
            else
                await BotService.SendMessageAsync(m.Chat, result.ErrorText!, token: token);
        }
    }
}
