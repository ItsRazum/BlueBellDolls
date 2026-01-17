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
}
