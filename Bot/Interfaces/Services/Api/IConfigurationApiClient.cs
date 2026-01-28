using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api
{
    public interface IConfigurationApiClient
    {
        Task<ServiceResult<PhotosLimitsResponse>> GetPhotosLimitsAsync(CancellationToken token = default);
    }
}