using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Services.Api
{
    public class ConfigurationApiClient(IHttpClientFactory httpClientFactory) : TinyApiClientBase(httpClientFactory), IConfigurationApiClient
    {
        public async Task<ServiceResult<PhotosLimitsResponse>> GetPhotosLimitsAsync(CancellationToken token)
        {
            var response = await HttpClient.GetAsync("admin/configuration/photos/limit", token);
            return await FromResponse<PhotosLimitsResponse>(response, token);
        }
    }
}
