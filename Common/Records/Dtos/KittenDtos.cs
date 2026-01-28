using BlueBellDolls.Common.Enums;

namespace BlueBellDolls.Common.Records.Dtos
{
    public record KittenMinimalDto(
        int Id,
        string? Name,
        bool IsEnabled);

    public record CreateKittenDto(
        int LitterId,
        string Name,
        bool IsMale,
        string Description,
        KittenClass Class,
        KittenStatus Status
    );

    public record UpdateKittenDto(
        string? Name = null,
        string? Description = null,
        bool? IsMale = null
    );

    public record KittenListDto(
        int Id,
        string? Name,
        DateOnly BirthDay,
        string? MainPhotoUrl,
        KittenClass Class,
        KittenStatus Status,
        string? Description,
        CatColorMinimalDto? CatColor,
        bool IsMale,
        char LitterLetter,
        int LitterId,
        bool IsEnabled
    );

    public record KittenDetailDto(
        int Id,
        string? Name,
        DateOnly BirthDay,
        bool IsMale,
        bool IsEnabled,
        string? Description,
        CatColorMinimalDto? CatColor,
        KittenClass Class,
        KittenStatus Status,
        List<PhotoDto> Photos,
        LitterMinimalDto Litter
    );

    public record UpdateKittenColorRequest(string Color);

    public record UpdateKittenClassRequest(KittenClass Class);

    public record UpdateKittenStatusRequest(KittenStatus Status);
}
