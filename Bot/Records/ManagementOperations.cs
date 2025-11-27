namespace BlueBellDolls.Bot.Records
{
    public record struct ManagementOperationResult<TResult>(bool Success, string? ErrorText = null, TResult? Result = null) where TResult : class;
    public record struct ManagementOperationResult(bool Success, string? ErrorText = null);
    public record class StructWrapper<T>(T Value) where T : struct;
}
