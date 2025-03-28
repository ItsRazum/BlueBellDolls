using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Commands
{
    public class ManageGeneticTestsCallback : CallbackHandler
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessagesHelperService _messagesHelperService;

        public ManageGeneticTestsCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IEntityHelperService entityHelperService,
            IMessagesHelperService messagesHelperService) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _entityHelperService = entityHelperService;
            _messagesHelperService = messagesHelperService;

            AddCommandHandler(CallbackDataProvider.GetManagePhotosCallback<ParentCat>(Enums.PhotosManagementMode.GeneticTests), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); // [0]Command, [1]EntityId
            var entityId = int.Parse(args.Last());

            var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<ParentCat>(entityId, token);

            if (entity == null || entity.Titles.Count == 0) return;
            await BotService.DeleteMessageAsync(c.Chat, c.MessageId, token);

            await _messagesHelperService.SendGeneticTestsManagementMessageAsync(c.Chat, entity, token);
        }
    }
}
