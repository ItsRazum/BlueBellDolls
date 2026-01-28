using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api
{
    public interface IFeedbackApiClient
    {
        Task<ServiceResult<BookingRequestDetailDto>> CloseFeedbackRequestAsync(int requestId, CancellationToken token = default);
    }
}