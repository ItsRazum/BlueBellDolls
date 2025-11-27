using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class DeletePhotosCallback : CallbackHandler
    {
        private readonly IManagementServicesFactory _managementServicesFactory;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IMessagesHelperService _messagesHelperService;

        public DeletePhotosCallback(
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

            AddCommandHandler(CallbackDataProvider.GetDeletePhotoCallback<ParentCat>(PhotosType.Photos), HandleCallbackAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetDeletePhotoCallback<Litter>(PhotosType.Photos), HandleCallbackAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetDeletePhotoCallback<Kitten>(PhotosType.Photos), HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token)
            where TEntity : class, IDisplayableEntity
        {

            var args = c.CallbackData.Split(CallbackArgsSeparator); //[0]Command, [1]Entity Id

            var managementService = _managementServicesFactory.GetEntityManagementService<TEntity>();
            var entity = await managementService.GetEntityAsync(int.Parse(args.Last()), token);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
                return;
            }

            await _messagesHelperService.SendDeletePhotosConfirmationAsync(c, entity, token);
        }
    }
}
