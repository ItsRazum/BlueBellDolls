using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks.Common
{
    public class FindColorCallback : CallbackHandler
    {
        private readonly ICatColorTreeService _catColorTreeService;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IManagementServicesFactory _managementServicesFactory;

        public FindColorCallback(
            IBotService botService, 
            IOptions<BotSettings> botSettings, 
            ICallbackDataProvider callbackDataProvider,
            ICatColorTreeService catColorTreeService,
            IMessagesProvider messagesProvider,
            IMessageParametersProvider messageParametersProvider,
            IManagementServicesFactory managementServicesFactory) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _catColorTreeService = catColorTreeService;
            _messagesProvider = messagesProvider;
            _messageParametersProvider = messageParametersProvider;
            _managementServicesFactory = managementServicesFactory;

            AddCommandHandler("findColorParentCat", HandleCallbackAsync<ParentCat>);
            AddCommandHandler("findColorKitten", HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : Cat
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator, StringSplitOptions.RemoveEmptyEntries);
            var entityId = int.Parse(args.Last());
            var managementService = _managementServicesFactory.GetCatManagementService<TEntity>();
            var result = await managementService.GetEntityAsync(entityId, token);

            if (result.StatusCode == StatusCodes.Status404NotFound)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
                return;
            }

            var catColorTree = await _catColorTreeService.GetCatColorTreeAsync(token);

            if (catColorTree == null)
            {
                await BotService.AnswerCallbackQueryAsync(
                    c.CallbackId, 
                    _messagesProvider.CreateApiGetEntityFailureMessage(), 
                    token: token);
                return;
            }

            var entity = result.Value!;

            if (args.Length < 3)
            {
                await UpdateColorPicker(entity, string.Empty, [.. catColorTree.Keys], c, token);
                return;
            }

            var buildedColor = args[1];
            if (buildedColor.Count(c => c == '_') == 2)
            {
                var finalColor = buildedColor.Replace("_", "").Replace("/", "").Trim();
                entity = await UpdateEntityColor<TEntity>(entityId, finalColor, c, token);
                if (entity != null)
                    await UpdateEntityForm(entity, c, token);
            }
            else
            {
                var colorParts = buildedColor.Split('_', StringSplitOptions.RemoveEmptyEntries);
                var colors = colorParts.Length switch
                {
                    1 => [.. catColorTree[colorParts[0]].Keys],
                    2 => catColorTree[colorParts[0]][colorParts[1]],
                    _ => []
                };
                await UpdateColorPicker(entity, buildedColor, colors, c, token);
            }
        }

        private async Task UpdateColorPicker<TEntity>(TEntity entity, string builtColor, string[] colors, CallbackQueryAdapter c, CancellationToken token) where TEntity : Cat
        {
            await BotService.EditOrSendNewMessageAsync(c.Chat, c.MessageId,
                _messageParametersProvider.GetColorPickerParameters(entity, builtColor, colors), token);
        }

        private async Task<TEntity?> UpdateEntityColor<TEntity>(int entityId, string color, CallbackQueryAdapter c, CancellationToken token) where TEntity : Cat
        {
            var managementService = _managementServicesFactory.GetCatManagementService<TEntity>();
            var result = await managementService.UpdateColorAsync(entityId, color, token);
            if (result.Success)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateColorSetSuccessfullyMessage(result.Value!.Color!.DisplayName), token: token);
                return result.Value!;
            }
            else
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, result.Message ?? _messagesProvider.CreateColorUpdateErrorMessage(), token: token);
                return null;
            }
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
