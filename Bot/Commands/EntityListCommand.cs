using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using CatColor = BlueBellDolls.Common.Models.CatColor;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;

namespace BlueBellDolls.Bot.Commands
{
    public class EntityListCommand : CommandHandler
    {
        private readonly IManagementServicesFactory _managementServicesFactory;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;
        private readonly InlineKeyboardsSettings _inlineKeyboardsSettings;

        public EntityListCommand(
            IBotService botService,
            IManagementServicesFactory managementServicesFactory,
            IMessageParametersProvider messageParametersProvider,
            IMessagesProvider messagesProvider,
            IOptions<BotSettings> botSettings)
            : base(botService)
        {
            _managementServicesFactory = managementServicesFactory;
            _messageParametersProvider = messageParametersProvider;
            _messagesProvider = messagesProvider;
            _inlineKeyboardsSettings = botSettings.Value.InlineKeyboardsSettings;

            AddCommandHandler("/catlist", HandleCommandAsync<ParentCat>);
            AddCommandHandler("/litterlist", HandleCommandAsync<Litter>);
            AddCommandHandler("/kittenlist", HandleCommandAsync<Kitten>);
            AddCommandHandler("/catcolorlist", HandleCommandAsync<CatColor>);
        }

        private async Task HandleCommandAsync<TEntity>(MessageAdapter m, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var result = await _managementServicesFactory
                .GetEntityManagementService<TEntity>()
                .GetByPageAsync(1, _inlineKeyboardsSettings.PageSize, token);

            if (result.Success)
            {
                var page = result.Value!;
                await BotService.SendMessageAsync(
                    m.Chat,
                    _messageParametersProvider.GetEntityListParameters(page.Items, Enums.ListUnitActionMode.Edit, (1, page.TotalPages, page.TotalItems)),
                    token);
            }
            else
            {
                await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreateUnknownErrorMessage(result.Message), token: token);
            }
        }
    }
}
