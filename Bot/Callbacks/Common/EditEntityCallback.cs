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

namespace BlueBellDolls.Bot.Callbacks.Common
{
    public class EditEntityCallback : CallbackHandler
    {
        private readonly IManagementServicesFactory _managementServicesFactory;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;

        public EditEntityCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IManagementServicesFactory managementServicesFactory,
            IMessageParametersProvider messageParametersProvider,
            IMessagesProvider messagesProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _managementServicesFactory = managementServicesFactory;
            _messageParametersProvider = messageParametersProvider;
            _messagesProvider = messagesProvider;

            AddCommandHandler(CallbackDataProvider.GetEditEntityCallback<ParentCat>(), HandleCommandAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetEditEntityCallback<Litter>(), HandleCommandAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetEditEntityCallback<Kitten>(), HandleCommandAsync<Kitten>);
            //CatColor имеет свой обработчик, см. Callbacks/CatColors/EditCatColorCallback.cs
        }

        private async Task HandleCommandAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var entityId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());
            var managementService = _managementServicesFactory.GetEntityManagementService<TEntity>();
            var entity = await managementService.GetEntityAsync(entityId, token);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
                return;
            }

            await BotService.EditOrSendNewMessageAsync(c.Chat, c.MessageId, _messageParametersProvider.GetEntityFormParameters(entity), token);
        }
    }
}
