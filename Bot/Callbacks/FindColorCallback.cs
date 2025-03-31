using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class FindColorCallback : CallbackHandler
    {
        private readonly Dictionary<string, Dictionary<string, string[]>> _catColors;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IDatabaseService _databaseService;
        private readonly IEntityHelperService _entityHelperService;

        public FindColorCallback(
            IBotService botService, 
            IOptions<BotSettings> botSettings, 
            ICallbackDataProvider callbackDataProvider,
            IOptions<EntitySettings> entitySettings, 
            IMessagesProvider messagesProvider,
            IMessageParametersProvider messageParametersProvider,
            IDatabaseService databaseService,
            IEntityHelperService entityHelperService) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _catColors = entitySettings.Value.CatColors;
            _messagesProvider = messagesProvider;
            _messageParametersProvider = messageParametersProvider;
            _databaseService = databaseService;
            _entityHelperService = entityHelperService;

            AddCommandHandler("findColorParentCat", HandleCallbackAsync<ParentCat>);
            AddCommandHandler("findColorKitten", HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : Cat
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator, StringSplitOptions.RemoveEmptyEntries);
            var entityId = int.Parse(args[^1]);
            var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(entityId, token);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
                return;
            }

            if (args.Length < 3)
            {
                await UpdateColorPicker(entity, string.Empty, _catColors.Keys.ToArray(), c, token);
                return;
            }

            var buildedColor = args[1];
            if (buildedColor.Count(c => c == '_') == 2)
            {
                var finalColor = buildedColor.Replace('_', ' ').Replace("/", "").Trim();
                entity = await UpdateEntityColor<TEntity>(entityId, finalColor, c, token);
                await UpdateEntityForm(entity, c, token);
            }
            else
            {
                var colorParts = buildedColor.Split('_', StringSplitOptions.RemoveEmptyEntries);
                var colors = colorParts.Length switch
                {
                    1 => _catColors[colorParts[0]].Keys.ToArray(),
                    2 => _catColors[colorParts[0]][colorParts[1]],
                    _ => Array.Empty<string>()
                };
                await UpdateColorPicker(entity, buildedColor, colors, c, token);
            }
        }

        private async Task UpdateColorPicker<TEntity>(TEntity entity, string builtColor, string[] colors, CallbackQueryAdapter c, CancellationToken token) where TEntity : Cat
        {
            await BotService.EditOrSendNewMessageAsync(c.Chat, c.MessageId,
                _messageParametersProvider.GetColorPickerParameters(entity, builtColor, colors), token);
        }

        private async Task<TEntity> UpdateEntityColor<TEntity>(int entityId, string color, CallbackQueryAdapter c, CancellationToken token) where TEntity : Cat
        {
            var updatedEntity = await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
            {
                var entity = await unit.GetRepository<TEntity>().GetByIdAsync(entityId, ct);
                ArgumentNullException.ThrowIfNull(entity);
                entity.Color = color;
                await unit.SaveChangesAsync(ct);
                return entity;
            }, token);

            await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateColorSetSuccessfullyMessage(color), token: token);
            return updatedEntity;
        }

        private async Task UpdateEntityForm<TEntity>(TEntity entity, CallbackQueryAdapter c, CancellationToken token) where TEntity : Cat
        {
            await BotService.EditOrSendNewMessageAsync(
                c.Chat,
                c.MessageId,
                _messageParametersProvider.GetEntityFormParameters(entity),
                token);
        }
    }
}
