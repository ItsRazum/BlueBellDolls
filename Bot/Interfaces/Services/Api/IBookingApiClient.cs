namespace BlueBellDolls.Bot.Interfaces.Services.Api
{
    public interface IBookingApiClient
    {
        Task<bool> CloseBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);
        Task<bool> ProcessBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);
    }
}