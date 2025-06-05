using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Services;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class NewEntityCommand : CommandHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IManagementService _managementService;

        public NewEntityCommand(
            IBotService botService,
            IMessageParametersProvider messageParametersProvider,
            IManagementService managementService)
            : base(botService)
        {
            _messageParametersProvider = messageParametersProvider;
            _managementService = managementService;

            AddCommandHandler("/newcat", HandleCommandAsync<ParentCat>);
            AddCommandHandler("/newlitter", HandleCommandAsync<Litter>);
        }

        private async Task HandleCommandAsync<TEntity>(MessageAdapter m, CancellationToken token) where TEntity : class, IDisplayableEntity, new()
        {
            var result = await _managementService.AddNewEntityAsync<TEntity>(token);

            if (result.Result != null)
                await BotService.SendMessageAsync(m.Chat, _messageParametersProvider.GetEntityFormParameters(result.Result), token);
            else
                await BotService.SendMessageAsync(m.Chat, result.ErrorText!, token: token);
        }
    }
}
