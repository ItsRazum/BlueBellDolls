using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using System.Linq.Expressions;

namespace BlueBellDolls.Bot.Commands
{
    public class EditEntityCallback : CommandHandler<CallbackQueryAdapter>
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;

        public EditEntityCallback(
            IBotService botService,
            IEntityHelperService entityHelperService,
            IMessageParametersProvider messageParametersProvider,
            IMessagesProvider messagesProvider)
            : base(botService)
        {
            _entityHelperService = entityHelperService;
            _messageParametersProvider = messageParametersProvider;
            _messagesProvider = messagesProvider;

            Handlers.Add("editParentCat", HandleCommandAsync<ParentCat>);
            Handlers.Add("editLitter", HandleCommandAsync<Litter>);
            Handlers.Add("editKitten", HandleCommandAsync<Kitten>);
        }

        private async Task HandleCommandAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            Expression<Func<TEntity, object?>>[] includes = typeof(TEntity) switch
            {
                _ when typeof(TEntity) == typeof(Litter) =>
                [
                    e => (e as Litter)!.Kittens,
                    e => (e as Litter)!.MotherCat,
                    e => (e as Litter)!.FatherCat
                ],
                _ => []
            };


            var entityId = int.Parse(c.CallbackData.Split('-').Last());
            var entity = await _entityHelperService.GetDisplayableEntityByIdAsync(entityId, token, includes);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
                return;
            }

            await BotService.EditOrSendNewMessageAsync(c.Chat, c.MessageId, _messageParametersProvider.GetEntityFormParameters(entity), token);
        }
    }
}
