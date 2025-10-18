using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class SetDefaultPhotoCallback : CallbackHandler
    {
        private readonly IMessagesHelperService _messagesHelperService;
        private readonly IArgumentParseHelperService _argumentParseHelperService;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementService _managementService;

        public SetDefaultPhotoCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessagesHelperService messagesHelperService,
            IArgumentParseHelperService argumentParseHelperService,
            IMessagesProvider messagesProvider,
            IManagementService managementService)
            : base(botService, botSettings, callbackDataProvider)
        {
            _messagesHelperService = messagesHelperService;
            _argumentParseHelperService = argumentParseHelperService;
            _messagesProvider = messagesProvider;
            _managementService = managementService;

            AddCommandHandler(CallbackDataProvider.GetSetDefaultPhotoCallback<ParentCat>(Enums.PhotosManagementMode.Photos), HandleCallbackAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetSetDefaultPhotoCallback<Litter>(Enums.PhotosManagementMode.Photos), HandleCallbackAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetSetDefaultPhotoCallback<Kitten>(Enums.PhotosManagementMode.Photos), HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token)
            where TEntity : class, IDisplayableEntity
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); //[0]Command, [1]PhotoIndex, [2]Entity Id
            var entityId = int.Parse(args.Last());
            var photoIndex = int.Parse(args[1]);
            var result = await _managementService.SetDefaultPhotoForEntityAsync<TEntity>(entityId, photoIndex, token);

            if (result.Success)
            {
                var entity = result.Result!;

                var key = c.MessageText.Split('\n').Last();
                var (_, photoMessageIds) = _argumentParseHelperService.ParsePhotosArgs(key);

                await BotService.DeleteMessagesAsync(c.Chat, [.. photoMessageIds, c.MessageId], token);
                await _messagesHelperService.SendPhotoManagementMessageAsync(c.Chat, entity, token);
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateDefaultPhotoSetForEntityMessage(entity, photoIndex), token: token);
            }
            else
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, result.ErrorText!, token: token);
            }
        }
    }
}
