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
    public class ConfirmDeleteGeneticTestsCallback : CallbackHandler
    {
        private readonly IArgumentParseHelperService _argumentParseHelperService;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementServicesFactory _managementServicesFactory;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesHelperService _messagesHelperService;

        public ConfirmDeleteGeneticTestsCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IArgumentParseHelperService argumentParseHelperService,
            IMessagesProvider messagesProvider,
            IManagementServicesFactory managementServicesFactory,
            IMessageParametersProvider messageParametersProvider,
            IMessagesHelperService messagesHelperService)
            : base(botService, botSettings, callbackDataProvider)
        {
            _argumentParseHelperService = argumentParseHelperService;
            _messagesProvider = messagesProvider;
            _managementServicesFactory = managementServicesFactory;
            _messageParametersProvider = messageParametersProvider;
            _messagesHelperService = messagesHelperService;

            AddCommandHandler(CallbackDataProvider.GetConfirmDeletePhotoCallback<ParentCat>(PhotosType.GenTests), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var (photoKeys, _) = _argumentParseHelperService.ParsePhotosArgs(c.MessageText.Split('\n').Last());
            var entityId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());

            var managementService = _managementServicesFactory.GetDisplayableEntityManagementService<ParentCat>();
            var result = await managementService.DeleteEntityPhotosAsync(entityId, [.. photoKeys], token);

            if (result.Success)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId,
                    _messagesProvider.CreatePhotosDeletionSuccessMessage(),
                    token: token);

                if (result.Result!.Photos.Count > 0)
                    await _messagesHelperService.SendPhotoManagementMessageAsync(c.Chat, result.Result, token);

                else
                    await BotService.EditOrSendNewMessageAsync(c.Chat, c.MessageId, _messageParametersProvider.GetEntityFormParameters(result.Result), token);
            }
            else
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId,
                    _messagesProvider.CreatePhotosDeletionFailureMessage(),
                    token: token);
            }
        }
    }
}
