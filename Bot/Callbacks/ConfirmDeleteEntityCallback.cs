using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Services;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class ConfirmDeleteEntityCallback : CallbackHandler
    {
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementService _managementService;

        public ConfirmDeleteEntityCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessagesProvider messagesProvider,
            IManagementService managementService)
            : base(botService, botSettings, callbackDataProvider)
        {
            _messagesProvider = messagesProvider;
            _managementService = managementService;

            AddCommandHandler(CallbackDataProvider.GetConfirmDeleteEntityCallback<ParentCat>(), HandleCallbackAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetConfirmDeleteEntityCallback<Litter>(), HandleCallbackAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetConfirmDeleteEntityCallback<Kitten>(), HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var entityId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());

            var result = await _managementService.DeleteEntityAsync<TEntity>(entityId, token);

            await BotService.AnswerCallbackQueryAsync(
                c.CallbackId, 
                result.Success 
                ? _messagesProvider.CreateEntityDeletionSuccess() 
                : result.ErrorText!,
                token: token);
        }
    }
}
