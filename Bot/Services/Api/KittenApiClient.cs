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

        public async Task<ServiceResult<List<KittenListDto>>> GetListAsync(KittenStatus? status = null, CancellationToken token = default)
        {
            var requestUrl = "/api/admin/kittens";
            if (status.HasValue)
                requestUrl += $"?status={status.Value}";

            try
            {
                var response = await HttpClient.GetAsync(requestUrl, token);
                return await FromResponse<List<KittenListDto>>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь список Kitten с сервера!", nameof(KittenApiClient), nameof(GetListAsync));
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<KittenDetailDto>> AddAsync(CreateKittenDto dto, CancellationToken token = default)
        {
            var requestUrl = "/api/admin/kittens";

            try
            {
                var response = await HttpClient.PostAsJsonAsync(requestUrl, dto, token);
                return await FromResponse<KittenDetailDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось добавить новую сущность Kitten!", nameof(KittenApiClient), nameof(AddAsync));
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<KittenDetailDto>> UpdateAsync(int id, UpdateKittenDto dto, CancellationToken token = default)
        {
            var requestUrl = $"/api/admin/kittens/{id}";

            try
            {
                var response = await HttpClient.PutAsJsonAsync(requestUrl, dto, token);
                return await FromResponse<KittenDetailDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить Kitten {id}!", nameof(KittenApiClient), nameof(UpdateAsync), id);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<PagedResult<KittenMinimalDto>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default)
        {
            var requestUrl = $"/api/admin/kittens?page={pageIndex}&pageSize={pageSize}";

            try
            {
                var response = await HttpClient.GetAsync(requestUrl, token);
                return await FromResponse<PagedResult<KittenMinimalDto>>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось получить страницу {p} списка Kitten!", nameof(KittenApiClient), nameof(GetByPageAsync), pageIndex);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<KittenDetailDto>> UpdateColorAsync(int entityId, string color, CancellationToken token)
        {
            var requestUrl = $"/api/admin/kittens/{entityId}/color";

            try
            {
                var request = new UpdateColorRequest(color);
                var response = await HttpClient.PutAsJsonAsync(requestUrl, request, token);
                return await FromResponse<KittenDetailDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить цвет для Kitten {id}!",
                    nameof(KittenApiClient), nameof(UpdateColorAsync), entityId);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<KittenDetailDto>> UpdateStatusAsync(int entityId, KittenStatus newStatus, CancellationToken token)
        {
            var requestUrl = $"/api/admin/kittens/{entityId}/status";

            try
            {
                var request = new UpdateKittenStatusRequest(newStatus);
                var response = await HttpClient.PostAsJsonAsync(requestUrl, request, token);
                return await FromResponse<KittenDetailDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить статус для Kitten {id}!",
                    nameof(KittenApiClient), nameof(UpdateStatusAsync), entityId);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<KittenDetailDto>> UpdateClassAsync(int entityId, KittenClass newClass, CancellationToken token)
        {
            var requestUrl = $"/api/admin/kittens/{entityId}/class";

            try
            {
                var request = new UpdateKittenClassRequest(newClass);
                var response = await HttpClient.PostAsJsonAsync(requestUrl, request, token);
                return await FromResponse<KittenDetailDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить класс для Kitten {id}!",
                    nameof(KittenApiClient), nameof(UpdateClassAsync), entityId);
                return new(500, "Неизвестная ошибка");
            }
        }
    }
}
