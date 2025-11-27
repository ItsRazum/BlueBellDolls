using BlueBellDolls.Common.Enums;

namespace BlueBellDolls.Common.Records.Dtos
{
    public record KittenMinimalDto(
        int Id,
        string Name,
        bool IsEnabled);

    public record CreateKittenDto(
        int LitterId,
        string Name,
        bool IsMale,
        string Description,
        string Color,
        KittenClass Class,
        KittenStatus Status
    );

    public record UpdateKittenDto(
        string Name,
        string Description,
        bool IsMale,
        string Color,
        KittenClass Class,
        KittenStatus Status
    );

    public record KittenListDto(
        int Id,
        string Name,
        DateOnly BirthDay,
        string? MainPhotoUrl,
        KittenStatus Status,
        string Description,
        string Color,
        bool IsMale,
        char LitterLetter,
        int LitterId
    );

    public record KittenDetailDto(
        int Id,
        string Name,
        DateOnly BirthDay,
        bool IsMale,
        bool IsEnabled,
        string Description,
        string Color,
        KittenClass Class,
        KittenStatus Status,
        List<PhotoDto> Photos,
        LitterSimpleDto Litter
    );
}
