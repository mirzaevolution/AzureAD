using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Client;

namespace AzureMSALWebApp.Helpers
{
    public class TokenCacheHelper
    {

        private static IDistributedCache _cache;
        private static ITokenCache _tokenCache;
        private static string _key;

        public static void Initialize(string key, IDistributedCache distributedCache, ITokenCache tokenCache)
        {

            _key = key;
            _cache = distributedCache;
            _tokenCache = tokenCache;
            _tokenCache.SetBeforeAccessAsync(OnBeforeAccessHandler);
            _tokenCache.SetAfterAccessAsync(OnAfterAccessHandler);
        }

        private static async Task OnAfterAccessHandler(TokenCacheNotificationArgs arg)
        {
            if (arg.HasStateChanged)
            {
                var data = arg.TokenCache.SerializeMsalV3();
                await _cache.SetAsync(_key, data);
            }
        }

        private static async Task OnBeforeAccessHandler(TokenCacheNotificationArgs arg)
        {
            var data = await _cache.GetAsync(_key);
            arg.TokenCache.DeserializeMsalV3(data);
        }
    }
}
