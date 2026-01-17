using BlueBellDolls.Bot.Records;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Interfaces.Management
{
    public interface IBookingManagementService
    {
        Task<ManagementOperationResult<BookingRequest>> CloseBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);
        Task<ManagementOperationResult<BookingRequest>> ProcessBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);
    }
}