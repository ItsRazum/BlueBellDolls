using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Commands
{
    public class NewEntityCommand : CommandHandler
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;

        public NewEntityCommand(
            IBotService botService,
            IEntityHelperService entityHelperService,
            IMessageParametersProvider messageParametersProvider)
            : base(botService)
        {
            _entityHelperService = entityHelperService;
            _messageParametersProvider = messageParametersProvider;

            AddCommandHandler("/newcat", HandleCommandAsync<ParentCat>);
            AddCommandHandler("/newlitter", HandleCommandAsync<Litter>);
        }

        private async Task HandleCommandAsync<TEntity>(MessageAdapter m, CancellationToken token) where TEntity : class, IDisplayableEntity, new()
        {
            var newEntity = await _entityHelperService.AddNewEntityAsync<TEntity>(token);
            await BotService.SendMessageAsync(m.Chat, _messageParametersProvider.GetEntityFormParameters(newEntity), token);
        }
    }
}
