using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Types;

namespace BlueBellDolls.Common.Records.Dtos
{
    public record PagedResult<T>(
        List<T> Items, 
        int PageNumber,
        int PageSize,
        int TotalItems,
        int TotalPages);

    public record UpdateColorRequest(string Color);

    public record PhotosLimitResponse(PhotosType PhotosType, int PhotosLimit);

    public record PhotosLimitsResponse(Dictionary<PhotosType, PhotosLimitsDictionary> Dictionaries);

    public record ServiceResult<T>(int StatusCode, string? Message = null, T? Value = null) 
        : ServiceResult(StatusCode, Message) where T : class;

    public record ServiceResult(int StatusCode, string? Message = null)
    {
        public bool Success => StatusCode >= 200 && StatusCode < 300;
    }

    public record ErrorResponse(string Message);

    public record StructWrapper<T>(T Value) where T : struct;
}
