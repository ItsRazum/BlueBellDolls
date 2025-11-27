using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class SelectParentCatCallback : CallbackHandler
    {
        private readonly IManagementServicesFactory _managementServicesFactory;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;
        private readonly InlineKeyboardsSettings _inlineKeyboardsSettings;

        public SelectParentCatCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IManagementServicesFactory managementServicesFactory,
            IMessageParametersProvider messageParametersProvider,
            IMessagesProvider messagesProvider) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _inlineKeyboardsSettings = botSettings.Value.InlineKeyboardsSettings;
            _managementServicesFactory = managementServicesFactory;
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

            var litter = await _managementServicesFactory
                .GetEntityManagementService<Litter>()
                .GetEntityAsync(litterId, token);

            if (litter == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(typeof(Litter), litterId), token: token);
                return;
            }

            var result = await _managementServicesFactory
                .GetParentCatManagementService()
                .GetByPageAsync(isMale, pageIndex, _inlineKeyboardsSettings.PageSize, token);

            if (result.Success)
            {
                var page = result.Result!;
                await BotService.EditOrSendNewMessageAsync(
                    c.Chat,
                    c.MessageId,
                    _messageParametersProvider.GetEntityListParameters(page.Items, Enums.ListUnitActionMode.Select, (pageIndex, page.TotalPages, page.TotalItems), litter),
                    token);
            }

        }
    }
}
