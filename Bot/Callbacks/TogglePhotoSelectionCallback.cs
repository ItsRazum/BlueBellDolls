using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class TogglePhotoSelectionCallback : CallbackHandler
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IArgumentParseHelperService _argumentParseHelperService;
        private readonly IMessagesProvider _messagesProvider;

        public TogglePhotoSelectionCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IEntityHelperService entityHelperService,
            IMessageParametersProvider messageParametersProvider,
            IArgumentParseHelperService argumentParseHelperService,
            IMessagesProvider messagesProvider) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _entityHelperService = entityHelperService;
            _messageParametersProvider = messageParametersProvider;
            _argumentParseHelperService = argumentParseHelperService;
            _messagesProvider = messagesProvider;

            AddCommandHandler(CallbackDataProvider.GetTogglePhotoSelectionCallback<ParentCat>(), HandleCallbackAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetTogglePhotoSelectionCallback<Kitten>(), HandleCallbackAsync<Kitten>);
            AddCommandHandler(CallbackDataProvider.GetTogglePhotoSelectionCallback<Litter>(), HandleCallbackAsync<Litter>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : IDisplayableEntity
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); //[0]Command, [1] PhotoIndex, [2]bool Select, [3] PhotosManagementMode, [4]EntityId
            var entityId = int.Parse(args.Last());

            var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(entityId, token);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
                return;
            }

            var key = c.MessageText.Split('\n').Last();
            var selectedPhotoIndex = int.Parse(args[1]);

            var (photoIndexes, photoMessageIds) = _argumentParseHelperService.ParsePhotosArgs(key);

            photoIndexes = bool.Parse(args[2]) 
                ? photoIndexes.Append(selectedPhotoIndex)
                : photoIndexes.Where(x => x != selectedPhotoIndex);

            var photoManagementMode = Enum.Parse<PhotosManagementMode>(args[3]);

            await BotService.EditMessageAsync(
                c.Chat, 
                c.MessageId, 
                _messageParametersProvider.GetEntityPhotosParameters(
                    entity,
                    photoManagementMode,
                    [..photoIndexes.Order()], 
                    [..photoMessageIds]),
                token);
        }
    }
}
