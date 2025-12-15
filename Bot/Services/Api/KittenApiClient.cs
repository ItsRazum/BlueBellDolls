using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Bot.Interfaces.Services.Api;

namespace BlueBellDolls.Bot.Services.Api
{
    public class KittenApiClient(IHttpClientFactory httpClientFactory, ILogger<KittenApiClient> logger) 
        : DisplayableEntityApiClientBase<Kitten>(httpClientFactory, logger), IKittenApiClient
    {
        private readonly ILogger<KittenApiClient> _logger = logger;

        public async Task<List<KittenListDto>?> GetListAsync(KittenStatus? status = null, CancellationToken token = default)
        {
            var url = "/api/admin/kittens";
            if (status.HasValue)
                url += $"?status={status.Value}";

            try
            {
                return await HttpClient.GetFromJsonAsync<List<KittenListDto>>(url, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь список Kitten с сервера!", nameof(KittenApiClient), nameof(GetListAsync));
                return null;
            }
        }

        public async Task<KittenDetailDto?> GetAsync(int id, CancellationToken token = default)
        {
            try
            {
                return await HttpClient.GetFromJsonAsync<KittenDetailDto>($"/api/admin/kittens/{id}", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь Kitten {id} с сервера!", nameof(KittenApiClient), nameof(GetAsync), id);
                return null;
            }
        }

        public async Task<KittenDetailDto?> AddAsync(CreateKittenDto dto, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync("/api/admin/kittens", dto, token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<KittenDetailDto>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось добавить новую сущность Kitten!", nameof(KittenApiClient), nameof(AddAsync));
                return null;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateKittenDto dto, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PutAsJsonAsync($"/api/admin/kittens/{id}", dto, token);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить Kitten {id}!", nameof(KittenApiClient), nameof(UpdateAsync), id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.DeleteAsync($"/api/admin/kittens/{id}", token);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось удалить Kitten {id}!", nameof(KittenApiClient), nameof(DeleteAsync), id);
                return false;
            }
        }

        public async Task<PagedResult<KittenMinimalDto>?> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default)
        {
            try
            {
                return await HttpClient.GetFromJsonAsync<PagedResult<KittenMinimalDto>>($"/api/admin/kittens?page={pageIndex}&pageSize={pageSize}", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось получить страницу {p} списка Kitten!", nameof(KittenApiClient), nameof(GetByPageAsync), pageIndex);
                return null;
            }
        }

        public async Task<bool> UpdateColorAsync(int entityId, string color, CancellationToken token)
        {
            var requestUrl = $"/api/admin/kittens/{entityId}/color";
            try
            {
                var response = await HttpClient.PutAsJsonAsync(requestUrl, new { Color = color }, token);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить цвет для Kitten {id}!",
                    nameof(KittenApiClient), nameof(UpdateColorAsync), entityId);
                return false;
            }
        }
    }
}
