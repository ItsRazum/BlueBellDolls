using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;

namespace BlueBellDolls.Bot.Callbacks.Common
{
    public class ConfirmDeleteEntityCallback : CallbackHandler
    {
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementServicesFactory _managementServicesFactory;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly InlineKeyboardsSettings _inlineKeyboardSettings;

        public ConfirmDeleteEntityCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessagesProvider messagesProvider,
            IManagementServicesFactory managementServicesFactory,
            IMessageParametersProvider messageParametersProvider,
            IOptions<InlineKeyboardsSettings> inlineKeyboardSettings)
            : base(botService, botSettings, callbackDataProvider)
        {
            _messagesProvider = messagesProvider;
            _managementServicesFactory = managementServicesFactory;
            _messageParametersProvider = messageParametersProvider;
            _inlineKeyboardSettings = inlineKeyboardSettings.Value;

            AddCommandHandler(CallbackDataProvider.GetConfirmDeleteEntityCallback<ParentCat>(), HandleCallbackAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetConfirmDeleteEntityCallback<Litter>(), HandleCallbackAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetConfirmDeleteEntityCallback<Kitten>(), HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var entityId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());

            var managementService = _managementServicesFactory.GetEntityManagementService<TEntity>();
            var result = await managementService.DeleteEntityAsync(entityId, token);

            await BotService.AnswerCallbackQueryAsync(
                c.CallbackId, 
                result.Success 
                ? _messagesProvider.CreateEntityDeletionSuccess() 
                : result.ErrorText!,
                token: token);

            var pageResult = await managementService.GetByPageAsync(1, _inlineKeyboardSettings.PageSize, token);
            if (pageResult.Success)
                await BotService.EditOrSendNewMessageAsync(
                    c.Chat, 
                    c.MessageId, 
                    _messageParametersProvider.GetEntityListParameters(
                        pageResult.Result!.Items,
                        Enums.ListUnitActionMode.Edit, 
                        (1, pageResult.Result.TotalPages, pageResult.Result.TotalItems)), 
                    token);
        }
    }
}
