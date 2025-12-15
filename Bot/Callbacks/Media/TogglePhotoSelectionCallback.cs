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
    public class TogglePhotoSelectionCallback : CallbackHandler
    {
        private readonly IManagementServicesFactory _managementServicesFactory;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IArgumentParseHelperService _argumentParseHelperService;
        private readonly IMessagesProvider _messagesProvider;

        public TogglePhotoSelectionCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IManagementServicesFactory managementServicesFactory,
            IMessageParametersProvider messageParametersProvider,
            IArgumentParseHelperService argumentParseHelperService,
            IMessagesProvider messagesProvider) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _managementServicesFactory = managementServicesFactory;
            _messageParametersProvider = messageParametersProvider;
            _argumentParseHelperService = argumentParseHelperService;
            _messagesProvider = messagesProvider;

            AddCommandHandler(CallbackDataProvider.GetTogglePhotoSelectionCallback<ParentCat>(), HandleCallbackAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetTogglePhotoSelectionCallback<Kitten>(), HandleCallbackAsync<Kitten>);
            AddCommandHandler(CallbackDataProvider.GetTogglePhotoSelectionCallback<Litter>(), HandleCallbackAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetTogglePhotoSelectionCallback<CatColor>(), HandleCallbackAsync<CatColor>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); //[0]Command, [1] PhotoIndex, [2]bool Select, [3] PhotosManagementMode, [4]EntityId
            var entityId = int.Parse(args.Last());

            var entity = await _managementServicesFactory
                .GetEntityManagementService<TEntity>()
                .GetEntityAsync(entityId, token);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
                return;
            }

            var key = c.MessageText.Split('\n').Last();
            var selectedPhotoId = int.Parse(args[1]);

            var (photoIds, photoMessageIds) = _argumentParseHelperService.ParsePhotosArgs(key);

            photoIds = bool.Parse(args[2]) 
                ? photoIds.Append(selectedPhotoId)
                : photoIds.Where(x => x != selectedPhotoId);

            var photoManagementMode = Enum.Parse<PhotosType>(args[3]);

            await BotService.EditMessageAsync(
                c.Chat, 
                c.MessageId, 
                _messageParametersProvider.GetEntityPhotosParameters(
                    entity,
                    photoManagementMode,
                    [..photoIds.Order()], 
                    [..photoMessageIds]),
                token);
        }
    }
}
