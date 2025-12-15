using BlueBellDolls.Bot.Interfaces.Management;
using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Records;

namespace BlueBellDolls.Bot.Services.Management
{
    public class BookingManagementService(
        IBookingApiClient bookingApiClient,
        ILogger<BookingManagementService> logger) : IBookingManagementService
    {
        private readonly IBookingApiClient _bookingApiClient = bookingApiClient;
        private readonly ILogger<BookingManagementService> _logger = logger;

        public async Task<ManagementOperationResult> ProcessBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default)
        {
            try
            {
                var result = await _bookingApiClient.ProcessBookingRequestAsync(bookingId, telegramUserId, token);

                return new ManagementOperationResult
                {
                    Success = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось сменить статус брони {bookingId}", nameof(BookingManagementService), nameof(ProcessBookingRequestAsync), bookingId);
                return new ManagementOperationResult
                {
                    Success = false,
                    ErrorText = ex.Message
                };
            }
        }

        public async Task<ManagementOperationResult> CloseBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default)
        {
            try
            {
                var result = await _bookingApiClient.CloseBookingRequestAsync(bookingId, telegramUserId, token);
                return new ManagementOperationResult
                {
                    Success = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось закрыть бронь {bookingId}", nameof(BookingManagementService), nameof(CloseBookingRequestAsync), bookingId);
                return new ManagementOperationResult
                {
                    Success = false,
                    ErrorText = ex.Message
                };
            }
        }
    }
}
