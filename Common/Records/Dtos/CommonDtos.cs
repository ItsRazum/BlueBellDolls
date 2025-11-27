namespace BlueBellDolls.Common.Records.Dtos
{
    public record PagedResult<T>(
        List<T> Items, 
        int PageNumber,
        int PageSize,
        int TotalItems,
        int TotalPages);

    public record UpdateColorRequest(string Color);
}
