using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Services;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class ConfirmDeletePhotosCallback : CallbackHandler
    {
        private readonly IArgumentParseHelperService _argumentParseHelperService;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementService _managementService;

        public ConfirmDeletePhotosCallback(
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

            AddCommandHandler(CallbackDataProvider.GetConfirmDeletePhotoCallback<ParentCat>(PhotosManagementMode.Photos), HandleCallbackAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetConfirmDeletePhotoCallback<Litter>(PhotosManagementMode.Photos), HandleCallbackAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetConfirmDeletePhotoCallback<Kitten>(PhotosManagementMode.Photos), HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : IDisplayableEntity
        {
            var (photoIndexes, _) = _argumentParseHelperService.ParsePhotosArgs(c.MessageText.Split('\n').Last());
            var entityId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());

            var result = await _managementService.DeleteEntityPhotosAsync<TEntity>(entityId, photoIndexes, PhotosManagementMode.Photos, token);

            await BotService.AnswerCallbackQueryAsync(c.CallbackId, result.Success
                ? _messagesProvider.CreatePhotosDeletionSuccessMessage()
                : _messagesProvider.CreatePhotosDeletionFailureMessage(),
                token: token);
        }
    }
}
