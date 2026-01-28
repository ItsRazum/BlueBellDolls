namespace BlueBellDolls.Common.Records.Dtos
{
    public record LitterMinimalDto(
        int Id,
        char Letter,
        bool IsEnabled
    );

    public record CreateLitterDto(
        DateOnly BirthDay,
        string Description,
        int? MotherCatId,
        int? FatherCatId
    );

    public record LitterSimpleDto(
        int Id,
        char Letter,
        bool IsEnabled,
        string? MotherName,
        string? FatherName
    );

    public record UpdateLitterDto(
        char? Letter = null,
        DateOnly? BirthDay = null,
        string? Description = null,
        int? MotherCatId = null,
        int? FatherCatId = null
    );

    public record LitterDetailDto(
        int Id,
        char Letter,
        DateOnly BirthDay,
        bool IsEnabled,
        string? Description,
        List<PhotoDto> Photos,
        int? MotherCatId,
        ParentCatMinimalDto? MotherCat,
        int? FatherCatId,
        ParentCatMinimalDto? FatherCat,
        List<KittenListDto> Kittens,
        int TotalKittens,
        int AvailableKittens
    );

    public record SetParentCatForLitterResponse(bool IsMale, LitterDetailDto Litter);
}
