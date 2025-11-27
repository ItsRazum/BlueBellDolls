namespace BlueBellDolls.Server.Records
{
    public record ServiceResult<T>(int StatusCode, string? Message = null, T? Value = null) where T : class;
    public record ServiceResult(int StatusCode, string? Message = null);
}
