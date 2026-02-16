using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;

namespace BlueBellDolls.Bot.Callbacks.ParentCats
{
    public class SelectParentCatCallback : CallbackHandler
    {
        private readonly IManagementServicesProvider _managementServicesProvider;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;
        private readonly InlineKeyboardsSettings _inlineKeyboardsSettings;

        public SelectParentCatCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IManagementServicesProvider managementServicesProvider,
            IMessageParametersProvider messageParametersProvider,
            IMessagesProvider messagesProvider) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _inlineKeyboardsSettings = botSettings.Value.InlineKeyboardsSettings;
            _managementServicesProvider = managementServicesProvider;
            _messageParametersProvider = messageParametersProvider;
            _messagesProvider = messagesProvider;

            AddCommandHandler(CallbackDataProvider.GetSelectEntityCallback<ParentCat>(), HandleCommandAsync);
        }

        private async Task HandleCommandAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); // [0]Command, [1]IsMale, [2]Page, [3]LitterId

            var isMale = bool.Parse(args[1]);
            var pageIndex = int.Parse(args[2]);
            var litterId = int.Parse(args.Last());

            var result = await _managementServicesProvider
                .GetParentCatManagementService()
                .GetByPageAsync(isMale, pageIndex, _inlineKeyboardsSettings.PageSize, token);

            if (result.Success)
            {
                var page = result.Value!;
                await BotService.EditOrSendNewMessageAsync(
                    c.Chat,
                    c.MessageId,
                    _messageParametersProvider.GetEntityListParameters(page.Items, Enums.ListUnitActionMode.Select, (pageIndex, page.TotalPages, page.TotalItems), litterId),
                    token);
            }

        }
    }
}
