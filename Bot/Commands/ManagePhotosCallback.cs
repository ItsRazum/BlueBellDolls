using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class ManagePhotosCallback : CommandHandler<CallbackQueryAdapter>
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessagesHelperService _messagesHelperService;

        public ManagePhotosCallback(
            IBotService botService,
            IEntityHelperService entityHelperService,
            IMessagesHelperService messagesHelperService)
            : base(botService)
        {
            _entityHelperService = entityHelperService;
            _messagesHelperService = messagesHelperService;

            Handlers.Add("managePhotosToParentCat", HandleCommandAsync<ParentCat>);
            Handlers.Add("managePhotosToKitten", HandleCommandAsync<Kitten>);
            Handlers.Add("managePhotosToLitter", HandleCommandAsync<Litter>);
        }

        private async Task HandleCommandAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : IDisplayableEntity
        {
            var args = c.CallbackData.Split('-'); // [0]Command, [1]EntityId
            var entityId = int.Parse(args.Last());

            var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(entityId, token);

            if (entity == null || entity.Photos.Count == 0) return;

            await BotService.DeleteMessageAsync(c.Chat, c.MessageId, token);

            await _messagesHelperService.SendPhotoManagementMessageAsync(c.Chat, entity, token);
        }
    }
}
