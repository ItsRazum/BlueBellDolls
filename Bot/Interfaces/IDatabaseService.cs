using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Interfaces
{
    internal interface IDatabaseService
    {
        Task ExecuteDbOperationAsync(Func<IUnitOfWork, CancellationToken, Task> operation, CancellationToken token);
        Task<TResult> GetDataAsync<TResult>(Func<IUnitOfWork, CancellationToken, Task<TResult>> operation, CancellationToken token) where TResult : class, new();
    }
}
