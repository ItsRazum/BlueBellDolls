using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Enums;
using CatColor = BlueBellDolls.Common.Models.CatColor;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;

namespace BlueBellDolls.Bot.Callbacks.Media
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
            AddCommandHandler(CallbackDataProvider.GetDeletePhotoCallback<CatColor>(PhotosType.Photos), HandleCallbackAsync<CatColor>);
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
