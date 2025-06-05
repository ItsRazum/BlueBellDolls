using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Services;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class ConfirmDeleteGeneticTestsCallback : CallbackHandler
    {
        private readonly IArgumentParseHelperService _argumentParseHelperService;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementService _managementService;

        public ConfirmDeleteGeneticTestsCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IArgumentParseHelperService argumentParseHelperService,
            IMessagesProvider messagesProvider,
            IManagementService managementService)
            : base(botService, botSettings, callbackDataProvider)
        {
            _argumentParseHelperService = argumentParseHelperService;
            _messagesProvider = messagesProvider;
            _managementService = managementService;

            AddCommandHandler(CallbackDataProvider.GetConfirmDeletePhotoCallback<ParentCat>(Enums.PhotosManagementMode.GenTests), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var (photoIndexes, _) = _argumentParseHelperService.ParsePhotosArgs(c.MessageText.Split('\n').Last());
            var entityId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());

            var result = await _managementService.DeleteEntityPhotosAsync<ParentCat>(entityId, photoIndexes, Enums.PhotosManagementMode.GenTests, token);

            await BotService.AnswerCallbackQueryAsync(c.CallbackId, result.Success
                ? _messagesProvider.CreatePhotosDeletionSuccessMessage()
                : _messagesProvider.CreatePhotosDeletionFailureMessage(),
                token: token);
        }
    }
}
