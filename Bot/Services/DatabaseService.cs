using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Data.Interfaces;

namespace BlueBellDolls.Bot.Services
{
    internal class DatabaseService(
        IServiceProvider serviceProvider,
        ILogger<DatabaseService> logger) : IDatabaseService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<DatabaseService> _logger = logger;

        public async Task ExecuteDbOperationAsync(Func<IUnitOfWork, CancellationToken, Task> operation, CancellationToken token)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            _logger.LogInformation("{DatabaseService}.{ExecuteDbOperationAsync}(): Открыто подключение к БД",
                nameof(DatabaseService),
                nameof(ExecuteDbOperationAsync));

            try
            {
                await operation(unitOfWork, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{DatabaseService}.{ExecuteDbOperationAsync}(): Возникло исключение", nameof(DatabaseService), nameof(ExecuteDbOperationAsync));
            }
        }

        public async Task<TResult> ExecuteDbOperationAsync<TResult>(Func<IUnitOfWork, CancellationToken, Task<TResult>> operation, CancellationToken token)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            _logger.LogInformation("{DatabaseService}.{ExecuteDbOperationAsync}<{TResult}>(): Открыто подключение к БД",
                nameof(DatabaseService),
                nameof(ExecuteDbOperationAsync),
                typeof(TResult).Name);

            try
            {
                return await operation(unitOfWork, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{DatabaseService}.{ExecuteDbOperationAsync}<{TResult}>(): Возникло исключение", 
                    nameof(DatabaseService), 
                    nameof(ExecuteDbOperationAsync), 
                    typeof(TResult).Name);
                throw;
            }
        }
    }
}
