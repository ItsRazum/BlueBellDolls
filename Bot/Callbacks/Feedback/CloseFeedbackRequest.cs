using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.Services.Management;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks.Feedback
{
    public class CloseFeedbackRequest : CallbackHandler
    {
        private readonly IFeedbackManagementService _feedbackManagementService;
        private readonly IMessagesProvider _messagesProvider;

        public CloseFeedbackRequest(
            IBotService botService, 
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IFeedbackManagementService feedbackManagementService,
            IMessagesProvider messagesProvider) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _feedbackManagementService = feedbackManagementService;
            _messagesProvider = messagesProvider;

            AddCommandHandler("closeFeedbackRequest", HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var feedbackRequestId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());
            var result = await _feedbackManagementService.CloseFeedbackRequestAsync(feedbackRequestId, token);
            if (result.Success)
            {
                await BotService.EditMessageAsync(
                    c.Chat.Id,
                    c.MessageId,
                    c.MessageText + $"\n[{DateTime.Now:g}] Запрос закрыт (@{c.From!.Username})",
                    token: token);
            }
            else
            {
                await BotService.AnswerCallbackQueryAsync(
                    c.CallbackId,
                    _messagesProvider.CreateUnknownErrorMessage(result.Message),
                    token: token);
            }
        }
    }
}
