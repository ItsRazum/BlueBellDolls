using BlueBellDolls.Common.Models;
using BlueBellDolls.Server.Records;

namespace BlueBellDolls.Server.Interfaces
{
    public interface IBookingService
    {
        Task<ServiceResult<BookingRequest>> AddBookingRequestAsync(string customerName, string customerPhone, int kittenId, CancellationToken token = default);

        Task<ServiceResult> ProcessBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);

        Task<ServiceResult> CloseBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);
    }
}
