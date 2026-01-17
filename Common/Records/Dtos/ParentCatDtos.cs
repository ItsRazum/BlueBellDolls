namespace BlueBellDolls.Common.Records.Dtos
{
    public record ParentCatMinimalDto(
        int Id,
        string Name,
        bool IsEnabled
    );

    public record CreateParentCatDto(
        string Name,
        DateOnly BirthDay,
        bool IsMale,
        string Description,
        string Color
    );

    public record UpdateParentCatDto(
        string Name,
        DateOnly BirthDay,
        bool IsMale,
        bool IsEnabled,
        string Description,
        string Color
    );

    public record ParentCatListDto(
        int Id,
        string Name,
        DateOnly BirthDay,
        bool IsMale,
        string Color,
        string Description,
        string? MainPhotoUrl,
        bool IsEnabled
    );

    public record ParentCatDetailDto(
        int Id,
        string Name,
        DateOnly BirthDay,
        bool IsMale,
        string Color,
        string Description,
        bool IsEnabled,
        List<PhotoDto> Photos
    );
}
