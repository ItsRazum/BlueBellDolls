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
    public class ToggleEntityVisibilityCallback : CallbackHandler
    {
        private readonly IMessagesProvider _messagesProvider;
        private readonly IKeyboardsProvider _keyboardsProvider;
        private readonly IManagementServicesProvider _managementServicesProvider;

        public ToggleEntityVisibilityCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessagesProvider messagesProvider,
            IManagementServicesProvider managementServicesProvider,
            IKeyboardsProvider keyboardsProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _messagesProvider = messagesProvider;
            _keyboardsProvider = keyboardsProvider;
            _managementServicesProvider = managementServicesProvider;

            AddCommandHandler(CallbackDataProvider.GetToggleEntityVisibilityCallback<ParentCat>(), HandleCallbackAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetToggleEntityVisibilityCallback<Litter>(), HandleCallbackAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetToggleEntityVisibilityCallback<Kitten>(), HandleCallbackAsync<Kitten>);
            AddCommandHandler(CallbackDataProvider.GetToggleEntityVisibilityCallback<CatColor>(), HandleCallbackAsync<CatColor>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var entityId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());
            var managementService = _managementServicesProvider.GetDisplayableEntityManagementService<TEntity>();
            var result = await managementService.ToggleEntityVisibilityAsync(entityId, token);

            if (!result.Success)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateServerErrorMessage(result.Message), token: token);
                return;
            }

            await BotService.EditInlineKeyboardAsync(c.Chat, c.MessageId, _keyboardsProvider.CreateEntityOptionsKeyboard(result.Value!), token);
            await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateToggleEntityVisibilitySuccessMessage(result.Value!), token: token);
        }
    }
}
