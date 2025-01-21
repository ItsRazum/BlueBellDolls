using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Services
{
    internal class DatabaseService : IDatabaseService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(
            IServiceProvider serviceProvider,
            ILogger<DatabaseService> logger) 
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteDbOperationAsync(Func<IUnitOfWork, CancellationToken, Task> operation, CancellationToken token)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            try
            {
                await operation(unitOfWork, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown in DatabaseService.ExecuteDbOperationAsync()");
            }
        }

        public async Task<TResult> GetDataAsync<TResult>(Func<IUnitOfWork, CancellationToken, Task<TResult>> operation, CancellationToken token) where TResult : class, new()
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            try
            {
                return await operation(unitOfWork, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown in DatabaseService.GetDataAsync<TResult>()");
                return new TResult();
            }
        }
    }
}
