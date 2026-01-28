using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Interfaces.Services.Management;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Services.Management
{
    public class FeedbackManagementService(
        IFeedbackApiClient apiClient,
        ILogger<FeedbackManagementService> logger) : IFeedbackManagementService
    {
        private readonly IFeedbackApiClient _apiClient = apiClient;
        private readonly ILogger<FeedbackManagementService> _logger = logger;

        public async Task<ServiceResult> CloseFeedbackRequestAsync(int feedbackRequestId, CancellationToken token)
        {
            try
            {
                var result = await _apiClient.CloseFeedbackRequestAsync(feedbackRequestId, token);

                return new(result.StatusCode, result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось закрыть запрос на обратную связь!");
                return new ServiceResult(StatusCodes.Status500InternalServerError, $"Не удалось закрыть запрос на обратную связь: {ex.Message}");
            }
        }
    }
}
