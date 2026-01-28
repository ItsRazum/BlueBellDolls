namespace BlueBellDolls.Common.Records.Dtos
{
    public record ParentCatMinimalDto(
        int Id,
        string? Name,
        bool IsEnabled
    );

    public record CreateParentCatDto(
        string? Name,
        DateOnly BirthDay,
        bool IsMale,
        string? Description
    );

    public record UpdateParentCatDto(
        string? Name = null,
        DateOnly? BirthDay = null,
        bool? IsMale = null,
        string? Description = null
    );

    public record ParentCatListDto(
        int Id,
        string? Name,
        DateOnly BirthDay,
        bool IsMale,
        CatColorMinimalDto? CatColor,
        string? Description,
        string? MainPhotoUrl,
        bool IsEnabled
    );

    public record ParentCatDetailDto(
        int Id,
        string? Name,
        DateOnly BirthDay,
        bool IsMale,
        CatColorMinimalDto? CatColor,
        string? Description,
        bool IsEnabled,
        List<PhotoDto> Photos
    );
}
