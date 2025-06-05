using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Services;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace BlueBellDolls.Bot.Callbacks
{
    public class SelectToLitterCallback : CallbackHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementService _managementService;
        private readonly IEntityHelperService _entityHelperService;

        public SelectToLitterCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessageParametersProvider messageParametersProvider,
            IMessagesProvider messagesProvider,
            IManagementService managementService,
            IEntityHelperService entityHelperService) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _messageParametersProvider = messageParametersProvider;
            _messagesProvider = messagesProvider;
            _managementService = managementService;
            _entityHelperService = entityHelperService;

            AddCommandHandler(CallbackDataProvider.GetSelectToLitterCallback(), HandleCommandAsync);
        }

        private async Task HandleCommandAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); // [0]Command, [1]LitterId, [2]EntityType, [3]EntityId

            var litterId = int.Parse(args[1]);
            var parentCatId = int.Parse(args[3]);

            var result = await _managementService.SetParentCatForLitterAsync(litterId, parentCatId, token);

            if (result.Success)
            {
                var parentCat = await _entityHelperService.GetDisplayableEntityByIdAsync<ParentCat>(parentCatId, token);
                ArgumentNullException.ThrowIfNull(parentCat);

                var litter = result.Result!;
                await BotService.AnswerCallbackQueryAsync(
                    c.CallbackId,
                    _messagesProvider.CreateParentCatSetForLitter(parentCat, litter),
                    token: token
                );

                await BotService.EditMessageAsync(
                    c.Chat,
                    c.MessageId,
                    _messageParametersProvider.GetEntityFormParameters(litter),
                    token
                );
            }

        }
    }
}
