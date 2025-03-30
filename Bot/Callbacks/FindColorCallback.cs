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
            string[] findedColors;
            var args = c.CallbackData.Split(CallbackArgsSeparator, StringSplitOptions.RemoveEmptyEntries); //[0]Command, [1]? Builded Color, [2] Entity Id
            var entityId = int.Parse(args.Last());
            TEntity? entity;

            if (args.Length < 3)
            {
                findedColors = [.. _catColors.Keys];
                entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(entityId, token);

                if (entity == null)
                {
                    await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
                    return;
                }

                await BotService.EditMessageAsync(c.Chat, c.MessageId, _messageParametersProvider.GetColorPickerParameters(entity, string.Empty, findedColors), token);

            }
            else if (args.Length == 3)
            {
                var buildedColor = args[1];
                if (buildedColor.Split('_').Length == 3)
                {
                    var finalColor = buildedColor.Replace('_', ' ').Replace("/", "").Trim();
                    entity = await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
                    {
                        var entity = await unit.GetRepository<TEntity>().GetByIdAsync(entityId, ct);
                        ArgumentNullException.ThrowIfNull(entity);

                        entity.Color = finalColor;

                        await unit.SaveChangesAsync(ct);
                        return entity;
                    }, token);

                    await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateColorSetSuccessfullyMessage(finalColor), token: token);
                    await BotService.EditMessageAsync(c.Chat, c.MessageId, _messageParametersProvider.GetEntityFormParameters(entity), token);
                }
                else
                {
                    var colorParts = buildedColor.Split('_', StringSplitOptions.RemoveEmptyEntries);
                    findedColors = colorParts.Length switch
                    {
                        1 => [.. _catColors[colorParts.First()].Keys],
                        2 => _catColors[colorParts.First()][colorParts.Last()],
                        _ => []
                    };

                    entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(entityId, token);

                    if (entity == null)
                    {
                        await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
                        return;
                    }

                    await BotService.EditMessageAsync(c.Chat, c.MessageId, _messageParametersProvider.GetColorPickerParameters(entity, buildedColor, findedColors), token);

                }
            }
        }
    }
}
