using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Common.Dtos;
using BlueBellDolls.Common.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace BlueBellDolls.Bot.Services
{
    public class CatColorTreeService(
        IMemoryCache memoryCache,
        ICatColorApiClient catColorApiClient) : ICatColorTreeService
    {
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly ICatColorApiClient _catColorApiClient = catColorApiClient;

        private const string _cacheKey = "CatColorTree";

        public async Task<CatColorTree?> GetCatColorTreeAsync(CancellationToken token = default)
        {
            if (_memoryCache.TryGetValue(_cacheKey, out CatColorTree? cachedTree))
            {
                return cachedTree;
            }
            var tree = await _catColorApiClient.GetCatColorTreeAsync(token);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));
            _memoryCache.Set(_cacheKey, tree, cacheEntryOptions);
            return tree;
        }

        public async Task<bool> ForceUpdateCatColorTree(CancellationToken token = default)
        {
            var tree = await _catColorApiClient.GetCatColorTreeAsync(token);
            if (tree == null)
            {
                return false;
            }
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));
            _memoryCache.Set(_cacheKey, tree, cacheEntryOptions);
            return true;
        }

        public async Task<string[]> GetCatColorMapAsync(CancellationToken token = default)
        {
            return (await GetCatColorTreeAsync(token))?.ToTreeMap() ?? [];
        }
    }
}
