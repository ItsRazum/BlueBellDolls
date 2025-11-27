using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Factories;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class ManageTitlesCallback : CallbackHandler
    {
        private readonly IManagementServicesFactory _managementServicesFactory;
        private readonly IMessagesHelperService _messagesHelperService;

        public ManageTitlesCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IManagementServicesFactory managementServicesFactory,
            IMessagesHelperService messagesHelperService)
            : base(botService, botSettings, callbackDataProvider)
        {
            _managementServicesFactory = managementServicesFactory;
            _messagesHelperService = messagesHelperService;

            AddCommandHandler(CallbackDataProvider.GetManagePhotosCallback<ParentCat>(PhotosType.Titles), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); // [0]Command, [1]EntityId
            var entityId = int.Parse(args.Last());

            var managementService = _managementServicesFactory.GetEntityManagementService<ParentCat>();
            var entity = await managementService.GetEntityAsync(entityId, token);

            if (entity == null || !entity.Photos.Where(p => p.Type == PhotosType.Titles).Any()) return;
            await BotService.DeleteMessageAsync(c.Chat, c.MessageId, token);

            await _messagesHelperService.SendTitlesManagementMessageAsync(c.Chat, entity, token);
        }
    }
}
