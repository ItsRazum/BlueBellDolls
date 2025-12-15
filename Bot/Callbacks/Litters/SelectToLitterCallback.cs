using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;

namespace BlueBellDolls.Bot.Callbacks.Litters
{
    public class SelectToLitterCallback : CallbackHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementServicesFactory _managementServicesFactory;

        public SelectToLitterCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessageParametersProvider messageParametersProvider,
            IMessagesProvider messagesProvider,
            IManagementServicesFactory managementServicesFactory)
            : base(botService, botSettings, callbackDataProvider)
        {
            _messageParametersProvider = messageParametersProvider;
            _messagesProvider = messagesProvider;
            _managementServicesFactory = managementServicesFactory;

            AddCommandHandler(CallbackDataProvider.GetSelectToLitterCallback(), HandleCommandAsync);
        }

        private async Task HandleCommandAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); // [0]Command, [1]LitterId, [2]EntityType, [3]EntityId

            var litterId = int.Parse(args[1]);
            var parentCatId = int.Parse(args[3]);

            var litterManagementService = _managementServicesFactory.GetLitterManagementService();
            var result = await litterManagementService.SetParentCatForLitterAsync(litterId, parentCatId, token);

            if (result.Success)
            {
                var parentCat = await _managementServicesFactory
                    .GetEntityManagementService<ParentCat>()
                    .GetEntityAsync(parentCatId, token);

                if (parentCat == null)
                {
                    await BotService.AnswerCallbackQueryAsync(
                        c.CallbackId,
                        _messagesProvider.CreateEntityNotFoundMessage(),
                        token: token
                    );
                    return;
                }

                if (result.Success)
                {
                    var litter = await litterManagementService.GetEntityAsync(litterId, token);
                    if (litter == null)
                    {
                        await BotService.AnswerCallbackQueryAsync(
                            c.CallbackId,
                            _messagesProvider.CreateApiGetEntityAfterUpdateFailureMessage(),
                            token: token
                        );
                        return;
                    }

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
}
