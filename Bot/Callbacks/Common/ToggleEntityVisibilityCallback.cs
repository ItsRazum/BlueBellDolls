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
        private readonly IManagementServicesFactory _managementServicesFactory;

        public ToggleEntityVisibilityCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessagesProvider messagesProvider,
            IManagementServicesFactory managementServicesFactory,
            IKeyboardsProvider keyboardsProvider) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _messagesProvider = messagesProvider;
            _keyboardsProvider = keyboardsProvider;
            _managementServicesFactory = managementServicesFactory;

            AddCommandHandler(CallbackDataProvider.GetToggleEntityVisibilityCallback<ParentCat>(), HandleCallbackAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetToggleEntityVisibilityCallback<Litter>(), HandleCallbackAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetToggleEntityVisibilityCallback<Kitten>(), HandleCallbackAsync<Kitten>);
            AddCommandHandler(CallbackDataProvider.GetToggleEntityVisibilityCallback<CatColor>(), HandleCallbackAsync<CatColor>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var entityId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());
            var managementService = _managementServicesFactory.GetDisplayableEntityManagementService<TEntity>();
            var result = await managementService.ToggleEntityVisibilityAsync(entityId, token);

            if (result.Success)
            {
                var entity = await managementService.GetEntityAsync(entityId, token);

                if (entity == null)
                {
                    await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
                    return;
                }

                await BotService.EditInlineKeyboardAsync(c.Chat, c.MessageId, _keyboardsProvider.CreateEntityOptionsKeyboard(entity), token);
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateToggleEntityVisibilitySuccessMessage(entity), token: token);
            }
            else
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, result.ErrorText!, token: token);
            }
        }
    }
}
