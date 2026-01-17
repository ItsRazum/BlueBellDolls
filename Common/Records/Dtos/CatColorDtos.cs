namespace BlueBellDolls.Common.Records.Dtos
{
    public record CatColorMinimalDto(
        int Id,
        string Identifier);

    public record CatColorDetailDto(
        int Id,
        string Identifier,
        string Description,
        bool IsEnabled,
        PhotoDto[] Photos);

    public record CreateCatColorDto(
        string Identifier,
        string Description);

    public record UpdateCatColorDto(
        string Identifier,
        string Description,
        bool Enabled);

    public record CatColorListDto(
        int Id,
        string Identifier,
        string Description,
        bool IsEnabled,
        string? MainPhotoUrl);
}
