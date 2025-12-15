using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using CatColor = BlueBellDolls.Common.Models.CatColor;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;

namespace BlueBellDolls.Bot.Callbacks.Common
{
    public class EntityListCallback : CallbackHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IManagementServicesFactory _managementServicesFactory;
        private readonly InlineKeyboardsSettings _inlineKeyboardsSettings;

        public EntityListCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IManagementServicesFactory managementServicesFactory,
            IMessageParametersProvider messageParametersProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _managementServicesFactory = managementServicesFactory;
            _messageParametersProvider = messageParametersProvider;
            _inlineKeyboardsSettings = botSettings.Value.InlineKeyboardsSettings;

            AddCommandHandler(CallbackDataProvider.GetListEntityCallback<ParentCat>(), HandleCommandAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetListEntityCallback<Litter>(), HandleCommandAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetListEntityCallback<Kitten>(), HandleCommandAsync<Kitten>);
            AddCommandHandler(CallbackDataProvider.GetListEntityCallback<CatColor>(), HandleCommandAsync<CatColor>);
        }

        public async Task HandleCommandAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); //[0]Command, [1]EntityId
            var messageFirstRow = c.MessageText.Split('\n').FirstOrDefault();
            if (messageFirstRow != null) 
            {
                var listArgs = messageFirstRow.Split(' '); //[0]ListUnitActionType, [1]EntityType, [2]? "для" [3]? OwnerType, [4]? OwnerId
                if (!Enum.TryParse<ListUnitActionMode>(listArgs[0], out var actionType))
                    actionType = ListUnitActionMode.Edit;

                // Проверяем, есть ли информация о владельце (например, список котят *для* помёта 5)
                // Если длина 5, значит есть владелец (индекс 4)
                var ownerId = listArgs.Length == 5
                    ? int.Parse(listArgs[4]) : 0; // ID владельца или 0, если нет

                var owner = ownerId == 0 
                    ? null 
                    : await _managementServicesFactory
                    .GetEntityManagementService<Litter>()
                    .GetEntityAsync(ownerId, token); //Могут быть проблемы, если владельцем в будущем сможет быть не только Litter

                var pageIndex = int.Parse(args.Last());
                var result = await _managementServicesFactory
                    .GetEntityManagementService<TEntity>()
                    .GetByPageAsync(pageIndex, _inlineKeyboardsSettings.PageSize, token);

                if (result.Success)
                {
                    var page = result.Result!;
                    await BotService.EditOrSendNewMessageAsync(
                        c.Chat,
                        c.MessageId,
                        _messageParametersProvider.GetEntityListParameters(page.Items, actionType, (pageIndex, page.TotalPages, page.TotalItems), owner),
                        token);
                }
            }
        }
    }
}
