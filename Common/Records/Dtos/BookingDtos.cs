namespace BlueBellDolls.Common.Records.Dtos
{
    public record BookingRequestDetailDto(int Id, string CustomerName, string CustomerPhone, int KittenId, long CuratorId, bool IsProcessed);
    public record CreateBookingRequestDto(string CustomerName, string CustomerPhone, int KittenId);
}
