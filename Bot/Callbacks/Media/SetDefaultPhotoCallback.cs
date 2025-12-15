using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Enums;
using CatColor = BlueBellDolls.Common.Models.CatColor;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.Providers;

namespace BlueBellDolls.Bot.Callbacks.Media
{
    public class SetDefaultPhotoCallback : CallbackHandler
    {
        private readonly IMessagesHelperService _messagesHelperService;
        private readonly IArgumentParseHelperService _argumentParseHelperService;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementServicesFactory _managementServicesFactory;

        public SetDefaultPhotoCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessagesHelperService messagesHelperService,
            IArgumentParseHelperService argumentParseHelperService,
            IManagementServicesFactory managementServicesFactory,
            IMessagesProvider messagesProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _messagesHelperService = messagesHelperService;
            _argumentParseHelperService = argumentParseHelperService;
            _managementServicesFactory = managementServicesFactory;
            _messagesProvider = messagesProvider;

            AddCommandHandler(CallbackDataProvider.GetSetDefaultPhotoCallback<ParentCat>(PhotosType.Photos), HandleCallbackAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetSetDefaultPhotoCallback<Litter>(PhotosType.Photos), HandleCallbackAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetSetDefaultPhotoCallback<Kitten>(PhotosType.Photos), HandleCallbackAsync<Kitten>);
            AddCommandHandler(CallbackDataProvider.GetSetDefaultPhotoCallback<CatColor>(PhotosType.Photos), HandleCallbackAsync<CatColor>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token)
            where TEntity : class, IDisplayableEntity
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); //[0]Command, [1]PhotoId, [2]Entity Id
            var entityId = int.Parse(args.Last());
            var photoId = int.Parse(args[1]);
            var managementService = _managementServicesFactory.GetDisplayableEntityManagementService<TEntity>();
            var result = await managementService.SetDefaultPhotoAsync(entityId, photoId, token);

            if (result.Success)
            {
                var entity = await managementService.GetEntityAsync(entityId, token);

                if (entity == null)
                {
                    await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
                    return;
                }

                var key = c.MessageText.Split('\n').Last();
                var (_, photoMessageIds) = _argumentParseHelperService.ParsePhotosArgs(key);

                await BotService.DeleteMessagesAsync(c.Chat, [.. photoMessageIds, c.MessageId], token);
                await _messagesHelperService.SendPhotoManagementMessageAsync(c.Chat, entity, token);
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateDefaultPhotoSetForEntityMessage(entity, photoId), token: token);
            }
            else
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, result.ErrorText!, token: token);
            }
        }
    }
}
