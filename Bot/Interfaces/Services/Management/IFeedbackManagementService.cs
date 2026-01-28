using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Management
{
    public interface IFeedbackManagementService
    {
        Task<ServiceResult> CloseFeedbackRequestAsync(int feedbackRequestId, CancellationToken token);
    }
}