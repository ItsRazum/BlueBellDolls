using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Commands
{
    public class AddGeneticTestsCommand : CommandHandler
    {
        private readonly IMessagesProvider _messagesProvider;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IDatabaseService _databaseService;
        private readonly IPhotosDownloaderService _photosDownloaderService;
        private readonly IEntityHelperService _entityHelperService;
        private readonly EntitySettings _entitySettings;

        public AddGeneticTestsCommand(
            IBotService botService,
            IMessagesProvider messagesProvider,
            IMessageParametersProvider messageParametersProvider,
            IDatabaseService databaseService,
            IPhotosDownloaderService photosDownloaderService,
            IEntityHelperService entityHelperService,
            IOptions<EntitySettings> options)
            : base(botService)
        {
            _messagesProvider = messagesProvider;
            _messageParametersProvider = messageParametersProvider;
            _databaseService = databaseService;
            _photosDownloaderService = photosDownloaderService;            
            _entityHelperService = entityHelperService;
            _entitySettings = options.Value;

            AddCommandHandler("генетический", HandleCommandAsync);
            AddCommandHandler("тесты", HandleCommandAsync);
        }

        private async Task HandleCommandAsync(MessageAdapter m, CancellationToken token)
        {
            if (m.ReplyToMessage == null) return;

            var entityArgs = m.ReplyToMessage.Text.Split('\n').First().Split(' '); //[0] ParentCat, [1] Entity Id

            if (entityArgs[0] != nameof(ParentCat)) return;
            if (m.Photos == null || m.Photos.Length is 0 or not (1 or 2)) return;

            var entityId = int.Parse(entityArgs.Last());
            var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<ParentCat>(entityId, token);
            if (entity == null)
            {
                await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
                return;
            }

            if (entity.GeneticTests.Count + m.Photos.Length > _entitySettings.MaxParentCatGeneticTestsCount)
            {
                await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreateGeneticTestsLimitReachedMessage(), token: token);
                return;
            }

            var loadingMessage = await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreatePhotosLoadingMessage(), token: token);

            var downloadedPhotos = await _photosDownloaderService.DownloadAndConvertPhotosToBase64(m.Photos, token);

            entity = await _databaseService.ExecuteDbOperationAsync(async (unit, ct) => //Присваивание нужно для обновления сущности
            {
                var entity = await unit.GetRepository<ParentCat>().GetByIdAsync(entityId, ct);
                ArgumentNullException.ThrowIfNull(entity);

                foreach(var photo in downloadedPhotos)
                    entity.GeneticTests.Add(photo.Key, photo.Value);

                await unit.SaveChangesAsync(ct);
                return entity;
            }, token);

            await BotService.DeleteMessagesAsync(m.Chat, [m.ReplyToMessage.MessageId, .. loadingMessage.Select(m => m.MessageId), .. m.Photos.Select(p => p.MessageId)], token);
            await BotService.SendMessageAsync(m.Chat, _messageParametersProvider.GetEntityFormParameters(entity), token);
        }
    }
}
