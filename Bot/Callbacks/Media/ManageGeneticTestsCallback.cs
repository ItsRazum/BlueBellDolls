using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;

namespace BlueBellDolls.Bot.Callbacks.Media
{
    public class ManageGeneticTestsCallback : CallbackHandler
    {
        private readonly IManagementServicesFactory _managementServicesFactory;
        private readonly IMessagesHelperService _messagesHelperService;

        public ManageGeneticTestsCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IManagementServicesFactory managementServicesFactory,
            IMessagesHelperService messagesHelperService) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _managementServicesFactory = managementServicesFactory;
            _messagesHelperService = messagesHelperService;

            AddCommandHandler(CallbackDataProvider.GetManagePhotosCallback<ParentCat>(PhotosType.GenTests), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); // [0]Command, [1]EntityId
            var entityId = int.Parse(args.Last());

            var managementService = _managementServicesFactory.GetEntityManagementService<ParentCat>();
            var entity = await managementService.GetEntityAsync(entityId, token);

            if (entity == null || !entity.Photos.Any(p => p.Type == PhotosType.GenTests)) return;
            await BotService.DeleteMessageAsync(c.Chat, c.MessageId, token);

            await _messagesHelperService.SendGeneticTestsManagementMessageAsync(c.Chat, entity, token);
        }
    }
}
