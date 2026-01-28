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
    public class ManagePhotosCallback : CallbackHandler
    {
        private readonly IManagementServicesFactory _managementServicesFactory;
        private readonly IMessagesHelperService _messagesHelperService;

        public ManagePhotosCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IManagementServicesFactory managementServicesFactory,
            IMessagesHelperService messagesHelperService)
            : base(botService, botSettings, callbackDataProvider)
        {
            _managementServicesFactory = managementServicesFactory;
            _messagesHelperService = messagesHelperService;

            AddCommandHandler(CallbackDataProvider.GetManagePhotosCallback<ParentCat>(PhotosType.Photos), HandleCommandAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetManagePhotosCallback<Kitten>(PhotosType.Photos), HandleCommandAsync<Kitten>);
            AddCommandHandler(CallbackDataProvider.GetManagePhotosCallback<Litter>(PhotosType.Photos), HandleCommandAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetManagePhotosCallback<CatColor>(PhotosType.Photos), HandleCommandAsync<CatColor>);
        }

        private async Task HandleCommandAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); // [0]Command, [1]EntityId
            var entityId = int.Parse(args.Last());

            var managementService = _managementServicesFactory.GetEntityManagementService<TEntity>();
            var result = await managementService.GetEntityAsync(entityId, token);

            if (!result.Success || result.Value?.Photos.Count is null or 0) return;

            await BotService.DeleteMessageAsync(c.Chat, c.MessageId, token);

            await _messagesHelperService.SendPhotoManagementMessageAsync(c.Chat, result.Value, token);
        }
    }
}
