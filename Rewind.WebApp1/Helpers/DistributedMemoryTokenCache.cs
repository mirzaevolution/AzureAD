using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Extensions.Caching.Distributed;
namespace Rewind.WebApp1.Helpers
{
    public class DistributedMemoryTokenCache : TokenCache
    {
        private readonly IDistributedCache _cache;
        private string _userId;
        public DistributedMemoryTokenCache(string userId, IDistributedCache distributedCache)
        {
            _userId = userId;
            _cache = distributedCache;
            this.BeforeAccess += BeforeAccessHandler;
            this.AfterAccess += AfterAccessHandler;
        }
        public override void DeleteItem(TokenCacheItem item)
        {
            base.DeleteItem(item);
            Persist();
        }
        public override void Clear()
        {
            base.Clear();
            _cache.Remove(_userId);
        }
        private void AfterAccessHandler(TokenCacheNotificationArgs args)
        {
            if (HasStateChanged)
            {
                Persist();
            }
        }

        private void BeforeAccessHandler(TokenCacheNotificationArgs args)
        {
            Load();
        }
        private void Persist()
        {

            _cache.Set(_userId, SerializeAdalV3());
            HasStateChanged = false;
        }
        private void Load()
        {

            var cacheData = _cache.Get(_userId);
            DeserializeAdalV3(cacheData);
        }
    }
}
