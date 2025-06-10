using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Commands
{
    public class AddPhotosCommand : CommandHandler
    {
        private readonly IDatabaseService _databaseService;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IEntityHelperService _entityHelperService;
        private readonly IPhotosDownloaderService _photosDownloaderService;
        private readonly EntitySettings _entitySettings;

        public AddPhotosCommand(
            IBotService botService,
            IDatabaseService databaseService,
            IMessageParametersProvider messageParametersProvider,
            IMessagesProvider messagesProvider,
            IEntityHelperService entityHelperService,
            IPhotosDownloaderService photosDownloaderService,
            IOptions<EntitySettings> entitySettings)
            : base(botService)
        {
            _databaseService = databaseService;
            _messageParametersProvider = messageParametersProvider;
            _messagesProvider = messagesProvider;
            _entityHelperService = entityHelperService;
            _photosDownloaderService = photosDownloaderService;
            _entitySettings = entitySettings.Value;

            AddCommandHandler("фото", HandleCommandAsync);
        }

        private async Task HandleCommandAsync(MessageAdapter m, CancellationToken token)
        {
            if (m.Photos == null) return;

            var repliedMessageText = m.ReplyToMessage!.Text;
            if (repliedMessageText == null) return;

            var messageArgs = repliedMessageText.Split('\n').First().Split(' ');
            var entityId = int.Parse(messageArgs[1]);
            var entityType = messageArgs.First();

            Func<PhotoAdapter[], int, CancellationToken, Task<(IDisplayableEntity? entity, bool photosLimitIsReached)>> addPhotosTask = entityType switch
            {
                "ParentCat" => AddPhotosToEntity<ParentCat>,
                "Kitten" => AddPhotosToEntity<Kitten>,
                "Litter" => AddPhotosToEntity<Litter>,
                _ => throw new InvalidDataException(entityType)
            };

            var loadingMessage = await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreatePhotosLoadingMessage(), token: token);

            var (entity, photosLimitIsReached) = await addPhotosTask(m.Photos, entityId, token);

            await BotService.DeleteMessageAsync(m.Chat, loadingMessage.Single().MessageId, token);
            if (entity != null)
            {
                if (photosLimitIsReached)
                    await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreatePhotosLimitReachedMessage(entity), token: token);
                else
                {
                    await BotService.DeleteMessagesAsync(m.Chat, [.. m.Photos.Select(p => p.MessageId), m.ReplyToMessage!.MessageId], token);
                    await BotService.SendMessageAsync(m.Chat, _messageParametersProvider.GetEntityFormParameters(entity), token);
                }
            }
        }

        private async Task<(IDisplayableEntity? entity, bool photosLimitIsReached)> AddPhotosToEntity<TEntity>(
            PhotoAdapter[] photos, int entityId, CancellationToken token) where TEntity : IDisplayableEntity
        {
            try
            {
                var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(entityId, token);
                if (entity == null) 
                    return (null, false);

                if (entity.Photos.Count + photos.Length > _entitySettings.MaxPhotos[entity.GetType().Name])
                    return (entity, true);

                var base64Photos = await _photosDownloaderService.DownloadAndConvertPhotosToBase64(photos, token);
                if (base64Photos.Count == 0) 
                    return (entity, false);

                return await _databaseService.ExecuteDbOperationAsync<(IDisplayableEntity?, bool)>(async (unit, ct) =>
                {
                    var entityFromDb = await unit.GetRepository<TEntity>().GetByIdAsync(entityId, ct);
                    if (entityFromDb == null) return (null, false);

                    if (entityFromDb.Photos.Count + base64Photos.Count > _entitySettings.MaxPhotos[entity.GetType().Name])
                        return (entityFromDb, true);

                    foreach (var photo in base64Photos)
                        entityFromDb.Photos.Add(photo.Key, photo.Value);

                    await unit.SaveChangesAsync(ct);
                    return (entityFromDb, false);

                }, token);
            }
            catch
            {
                return (null, false);
            }
        }
    }
}