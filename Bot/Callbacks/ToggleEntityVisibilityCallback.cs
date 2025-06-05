using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class ToggleEntityVisibilityCallback : CallbackHandler
    {
        private readonly IMessagesProvider _messagesProvider;
        private readonly IKeyboardsProvider _keyboardsProvider;
        private readonly IManagementService _managementService;

        public ToggleEntityVisibilityCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessagesProvider messagesProvider,
            IKeyboardsProvider keyboardsProvider,
            IManagementService managementService) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _messagesProvider = messagesProvider;
            _keyboardsProvider = keyboardsProvider;
            _managementService = managementService;

            AddCommandHandler(CallbackDataProvider.GetToggleEntityVisibilityCallback<ParentCat>(), HandleCallbackAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetToggleEntityVisibilityCallback<Litter>(), HandleCallbackAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetToggleEntityVisibilityCallback<Kitten>(), HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var entityId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());
            var result = await _managementService.ToggleEntityVisibilityAsync<TEntity>(entityId, token);

            if (result.Success)
            {
                await BotService.EditInlineKeyboardAsync(c.Chat, c.MessageId, _keyboardsProvider.CreateEntityOptionsKeyboard(result.Result!), token);
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateToggleEntityVisibilitySuccessMessage(result.Result!), token: token);
            }
            else
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, result.ErrorText!, token: token);
            }
        }
    }
}
