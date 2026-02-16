using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.Providers;

namespace BlueBellDolls.Bot.Callbacks.Media
{
    public class ConfirmDeleteTitlesCallback : CallbackHandler
    {
        private readonly IArgumentParseHelperService _argumentParseHelperService;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementServicesProvider _managementServicesProvider;

        public ConfirmDeleteTitlesCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IArgumentParseHelperService argumentParseHelperService,
            IMessagesProvider messagesProvider,
            IManagementServicesProvider managementServicesProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _argumentParseHelperService = argumentParseHelperService;
            _messagesProvider = messagesProvider;
            _managementServicesProvider = managementServicesProvider;

            AddCommandHandler(CallbackDataProvider.GetConfirmDeletePhotoCallback<ParentCat>(PhotosType.Titles), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var (photoKeys, _) = _argumentParseHelperService.ParsePhotosArgs(c.MessageText.Split('\n').Last());
            var entityId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());

            var managementService = _managementServicesProvider.GetDisplayableEntityManagementService<ParentCat>();
            var result = await managementService.DeleteEntityPhotosAsync(entityId, [.. photoKeys], token);

            await BotService.AnswerCallbackQueryAsync(c.CallbackId, result.Success
                ? _messagesProvider.CreatePhotosDeletionSuccessMessage()
                : _messagesProvider.CreatePhotosDeletionFailureMessage(),
                token: token);
        }
    }
}
