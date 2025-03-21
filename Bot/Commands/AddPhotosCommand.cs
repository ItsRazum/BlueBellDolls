using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Services;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Commands
{
    public class AddPhotosCommand : CommandHandler<MessageAdapter>
    {
        private readonly IDatabaseService _databaseService;
        private readonly IMessageParametersProvider _messageParametersProvider;

        private readonly BotSettings _botSettings;
        private readonly TelegramFilesHttpClientSettings _telegramFilesHttpClientSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public AddPhotosCommand(
            IBotService botService,
            IDatabaseService databaseService,
            IMessageParametersProvider messageParametersProvider,
            IOptions<TelegramFilesHttpClientSettings> telegramFilesHttpClientSettings,
            IOptions<BotSettings> botSettings,
            IHttpClientFactory httpClientFactory)
            : base(botService)
        {
            _databaseService = databaseService;
            _messageParametersProvider = messageParametersProvider;
            _telegramFilesHttpClientSettings = telegramFilesHttpClientSettings.Value;
            _botSettings = botSettings.Value;
            _httpClientFactory = httpClientFactory;

            Handlers.Add("Фото", HandleCommandAsync);
        }

        private async Task HandleCommandAsync(MessageAdapter m, CancellationToken token)
        {
            if (m.Photos == null) return;

            if (m.Photos.Length > 5)
            {
                await BotService.SendMessageAsync(m.Chat, "Количество фотографий не может быть больше 5!", token: token);
                return;
            }

            var repliedMessageText = m.ReplyToMessage!.Text;
            if (repliedMessageText == null) return;

            var messageArgs = repliedMessageText.Split('\n').First().Split(' ');
            var entityId = int.Parse(messageArgs[1]);
            var entityType = messageArgs.First();

            Func<PhotoAdapter[], int, CancellationToken, Task<IDisplayableEntity?>> addPhotosTask = entityType switch
            {
                "ParentCat" => AddPhotosToEntity<ParentCat>,
                "Kitten" => AddPhotosToEntity<Kitten>,
                "Litter" => AddPhotosToEntity<Litter>,
                _ => throw new InvalidDataException(entityType)
            };

            var loadingMessage = await BotService.SendMessageAsync(m.Chat, "Загрузка...", token: token);

            var entity = await addPhotosTask(m.Photos, entityId, token);

            await BotService.DeleteMessageAsync(m.Chat, loadingMessage.Single().MessageId, token);
            if (entity != null)
            {
                await BotService.DeleteMessagesAsync(m.Chat, [ ..m.Photos.Select(p => p.MessageId), m.ReplyToMessage!.MessageId], token);
                await BotService.SendMessageAsync(m.Chat, _messageParametersProvider.GetEntityFormParameters(entity), token);
            }
        }

        private async Task<IDisplayableEntity?> AddPhotosToEntity<TEntity>(PhotoAdapter[] photos, int entityId, CancellationToken token) where TEntity : IDisplayableEntity
        {
            try
            {
                var base64Photos = new Dictionary<string, string>();

                using (var httpClient = _httpClientFactory.CreateClient(_telegramFilesHttpClientSettings.ClientName))
                {
                    foreach (var photoAdapter in photos)
                    {
                        var file = await BotService.GetFileAsync(photoAdapter.FileId, token);

                        var response = await httpClient.GetAsync(
                            $"https://api.telegram.org/file/bot{_botSettings.Token}/{file.FilePath}",
                            HttpCompletionOption.ResponseHeadersRead,
                            token);

                        response.EnsureSuccessStatusCode();

                        await using var stream = await response.Content.ReadAsStreamAsync(token);
                        using var memoryStream = new MemoryStream();
                        await stream.CopyToAsync(memoryStream, token);

                        base64Photos.Add(photoAdapter.FileId, Convert.ToBase64String(memoryStream.ToArray()));
                    }
                }

                return await _databaseService.ExecuteDbOperationAsync<IDisplayableEntity?>(async (unit, ct) =>
                {
                    var entity = await unit.GetRepository<TEntity>().GetByIdAsync(entityId, ct);
                    if (entity != null)
                    {
                        foreach (var photo in base64Photos)
                            entity.Photos.Add(photo.Key, photo.Value);

                        await unit.SaveChangesAsync(ct);
                        return entity;
                    }

                    return null;
                }, token);
            }
            catch
            {
                return null;
            }
        }
    }
}