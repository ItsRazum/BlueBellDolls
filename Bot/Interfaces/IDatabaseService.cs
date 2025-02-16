using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IDatabaseService
    {
        Task ExecuteDbOperationAsync(Func<IUnitOfWork, CancellationToken, Task> operation, CancellationToken token);
        Task<TResult> ExecuteDbOperationAsync<TResult>(Func<IUnitOfWork, CancellationToken, Task<TResult>> operation, CancellationToken token);
    }
}
