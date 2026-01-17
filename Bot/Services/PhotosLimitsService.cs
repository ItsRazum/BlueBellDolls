using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Types;
using Microsoft.Extensions.Options;
using Timer = System.Timers.Timer;

namespace BlueBellDolls.Bot.Services
{
    public class PhotosLimitsService : IPhotosLimitsService
    {
        private readonly Timer _timer;
        private readonly PhotosLimitsServiceSettings _settings;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<PhotosLimitsService> _logger;
        private Dictionary<PhotosType, PhotosLimitsDictionary> _limits = [];

        public PhotosLimitsService(
            IOptions<PhotosLimitsServiceSettings> photosLimitsServicesettings,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<PhotosLimitsService> logger)
        {
            _settings = photosLimitsServicesettings.Value;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

            _timer = new()
            {
                Interval = TimeSpan.FromMinutes(_settings.UpdateIntervalMinutes).TotalMilliseconds,
                AutoReset = true,
            };
            _timer.Elapsed += Timer_Elapsed;
            _ = UpdateLimitsAsync();
            _timer.Start();
        }

        public int GetLimit<TEntity>(PhotosType photosType) where TEntity : class, IDisplayableEntity
        {
            if (_limits.TryGetValue(photosType, out var subDict))
                return subDict.SafeGet(typeof(TEntity).Name);

            return 0;
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _ = UpdateLimitsAsync();
        }

        private async Task UpdateLimitsAsync()
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var apiClient = scope.ServiceProvider.GetService<IConfigurationApiClient>();
                if (apiClient != null)
                {
                    var result = await apiClient.GetPhotosLimitsAsync();
                    if (result != null)
                        _limits = result.Dictionaries;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить лимиты фотографий из API!");
            }
        }
    }
}
