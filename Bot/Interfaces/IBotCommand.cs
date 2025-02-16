namespace BlueBellDolls.Bot.Interfaces
{
    public interface IBotCommand
    {
        Task<bool> ExecuteAsync(ICommandAdapter adapter, CancellationToken token);
    }
}
