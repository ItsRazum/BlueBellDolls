namespace BlueBellDolls.Common.Records.Dtos
{
    public record BookingRequestDetailDto(int Id, string Name, string PhoneNumber, int KittenId, long CuratorId, bool IsProcessed);
    public record CreateBookingRequestDto(string Name, string PhoneNumber, int KittenId);
}
