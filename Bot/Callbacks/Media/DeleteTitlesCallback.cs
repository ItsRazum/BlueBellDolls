using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks.Media
{
    public class DeleteTitlesCallback : CallbackHandler
    {
        private readonly IManagementServicesFactory _managementServicesFactory;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IMessagesHelperService _messagesHelperService;

        public DeleteTitlesCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IManagementServicesFactory managementServicesFactory,
            IMessagesProvider messagesProvider,
            IMessagesHelperService messagesHelperService)
            : base(botService, botSettings, callbackDataProvider)
        {
            _managementServicesFactory = managementServicesFactory;
            _messagesProvider = messagesProvider;
            _messagesHelperService = messagesHelperService;

            AddCommandHandler(CallbackDataProvider.GetDeletePhotoCallback<ParentCat>(PhotosType.Titles), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {

            var args = c.CallbackData.Split(CallbackArgsSeparator); //[0]Command, [1]Entity Id

            var managementService = _managementServicesFactory.GetEntityManagementService<ParentCat>();
            var result = await managementService.GetEntityAsync(int.Parse(args.Last()), token);

            if (!result.Success)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
                return;
            }

            await _messagesHelperService.SendDeleteTitlesConfirmationAsync(c, result.Value!, token);
        }
    }
}
