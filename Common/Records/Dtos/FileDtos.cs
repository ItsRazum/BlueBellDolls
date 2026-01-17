using BlueBellDolls.Common.Enums;

namespace BlueBellDolls.Common.Records.Dtos
{
    public record EntityFilesUploadResult<T>(T Dto, FileUploadResult[] Results);
    public record FileUploadResult(int FileIndex, bool Uploaded);
    public record PhotoDto(int Id, string Url, PhotosType Type, bool IsMain, TelegramPhotoDto? TelegramPhoto);
    public record TelegramPhotoDto(int Id, string FileId);
}
