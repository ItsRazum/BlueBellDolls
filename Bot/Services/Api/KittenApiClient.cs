using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Services.Api
{
    public class KittenApiClient(IHttpClientFactory httpClientFactory, ILogger<KittenApiClient> logger) 
        : DisplayableEntityApiClientBase<Kitten, KittenDetailDto>(httpClientFactory, logger), IKittenApiClient
    {
        private readonly ILogger<KittenApiClient> _logger = logger;

        public async Task<List<KittenListDto>?> GetListAsync(KittenStatus? status = null, CancellationToken token = default)
        {
            var requestUrl = "/api/admin/kittens";
            if (status.HasValue)
                requestUrl += $"?status={status.Value}";

            try
            {
                return await HttpClient.GetFromJsonAsync<List<KittenListDto>>(requestUrl, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь список Kitten с сервера!", nameof(KittenApiClient), nameof(GetListAsync));
                return null;
            }
        }

        public async Task<KittenDetailDto?> AddAsync(CreateKittenDto dto, CancellationToken token = default)
        {
            var requestUrl = "/api/admin/kittens";

            try
            {
                var response = await HttpClient.PostAsJsonAsync(requestUrl, dto, token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<KittenDetailDto>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось добавить новую сущность Kitten!", nameof(KittenApiClient), nameof(AddAsync));
                return null;
            }
        }

        public async Task<KittenDetailDto?> UpdateAsync(int id, UpdateKittenDto dto, CancellationToken token = default)
        {
            var requestUrl = $"/api/admin/kittens/{id}";

            try
            {
                var response = await HttpClient.PutAsJsonAsync(requestUrl, dto, token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<KittenDetailDto?>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить Kitten {id}!", nameof(KittenApiClient), nameof(UpdateAsync), id);
                return null;
            }
        }

        public async Task<PagedResult<KittenMinimalDto>?> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default)
        {
            var requestUrl = $"/api/admin/kittens?page={pageIndex}&pageSize={pageSize}";

            try
            {
                return await HttpClient.GetFromJsonAsync<PagedResult<KittenMinimalDto>>(requestUrl, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось получить страницу {p} списка Kitten!", nameof(KittenApiClient), nameof(GetByPageAsync), pageIndex);
                return null;
            }
        }

        public async Task<KittenDetailDto?> UpdateColorAsync(int entityId, string color, CancellationToken token)
        {
            var requestUrl = $"/api/admin/kittens/{entityId}/color";

            try
            {
                var request = new UpdateColorRequest(color);
                var response = await HttpClient.PutAsJsonAsync(requestUrl, request, token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<KittenDetailDto>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить цвет для Kitten {id}!",
                    nameof(KittenApiClient), nameof(UpdateColorAsync), entityId);
                return null;
            }
        }

        public async Task<KittenDetailDto?> UpdateStatusAsync(int entityId, KittenStatus newStatus, CancellationToken token)
        {
            var requestUrl = $"/api/admin/kittens/{entityId}/status";

            try
            {
                var request = new UpdateKittenStatusRequest(newStatus);
                var response = await HttpClient.PostAsJsonAsync(requestUrl, request, token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<KittenDetailDto>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить статус для Kitten {id}!",
                    nameof(KittenApiClient), nameof(UpdateColorAsync), entityId);
                return null;
            }
        }

        public async Task<KittenDetailDto?> UpdateClassAsync(int entityId, KittenClass newClass, CancellationToken token)
        {
            var requestUrl = $"/api/admin/kittens/{entityId}/class";

            try
            {
                var request = new UpdateKittenClassRequest(newClass);
                var response = await HttpClient.PostAsJsonAsync(requestUrl, request, token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<KittenDetailDto>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить класс для Kitten {id}!",
                    nameof(KittenApiClient), nameof(UpdateColorAsync), entityId);
                return null;
            }
        }
    }
}
