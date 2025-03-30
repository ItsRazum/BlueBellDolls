using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class ConfirmDeleteGeneticTestsCallback : CallbackHandler
    {
        private readonly IArgumentParseHelperService _argumentParseHelperService;
        private readonly IDatabaseService _databaseService;
        private readonly IMessagesProvider _messagesProvider;

        public ConfirmDeleteGeneticTestsCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IArgumentParseHelperService argumentParseHelperService,
            IDatabaseService databaseService,
            IMessagesProvider messagesProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _argumentParseHelperService = argumentParseHelperService;
            _databaseService = databaseService;
            _messagesProvider = messagesProvider;

            AddCommandHandler(CallbackDataProvider.GetConfirmDeletePhotoCallback<ParentCat>(Enums.PhotosManagementMode.GenTests), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var (photoIndexes, photoMessageIds) = _argumentParseHelperService.ParsePhotosArgs(c.MessageText.Split('\n').Last());
            var entityId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());

            var success = await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
            {
                var entity = await unit.GetRepository<ParentCat>().GetByIdAsync(entityId, ct);
                if (entity != null)
                {
                    var keysToRemove = photoIndexes
                    .Select(x => entity.GeneticTests.Keys.ElementAt(x))
                    .ToList();

                    foreach (var photoKey in keysToRemove)
                        entity.GeneticTests.Remove(photoKey);

                    await unit.SaveChangesAsync(token);

                    return true;
                }

                return false;
            }, token);


            await BotService.AnswerCallbackQueryAsync(c.CallbackId, success
                ? _messagesProvider.CreatePhotosDeletionSuccessMessage()
                : _messagesProvider.CreatePhotosDeletionFailureMessage(),
                token: token);
        }
    }
}
