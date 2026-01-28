using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Server.Interfaces
{
    public interface IFeedbackProcessingService
    {
        Task<ServiceResult<FeedbackRequestDetailDto>> AddFeedbackRequestAsync(CreateFeedbackRequestDto feedbackRequest, CancellationToken token);
        Task<ServiceResult<FeedbackRequestDetailDto>> MarkFeedbackRequestAsProcessedAsync(int feedbackRequestId, CancellationToken token);
    }
}