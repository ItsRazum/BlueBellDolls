using BlueBellDolls.Bot.Records;

namespace BlueBellDolls.Bot.Interfaces.Management
{
    public interface IBookingManagementService
    {
        Task<ManagementOperationResult> CloseBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);
        Task<ManagementOperationResult> ProcessBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);
    }
}