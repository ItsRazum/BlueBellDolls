namespace BlueBellDolls.Common.Records.Dtos
{
    public record CreateFeedbackRequestDto(string Name, string PhoneNumber);

    public record FeedbackRequestDetailDto(int Id, string Name, string Phone, bool IsProcessed);
}
