using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
namespace AzureADMSAL.Helpers
{
    public class DistributedMemoryTokenCache : TokenCache
    {
        private IDistributedCache _cacheHelper;
        private string _userId;
        private static object _syncLock = new object();
        public DistributedMemoryTokenCache(string userId, IDistributedCache distributedCache)
        {
            _userId = userId + "_TokenCache";
            _cacheHelper = distributedCache;
            InitializeComponents();
        }
        private void InitializeComponents()
        {
            this.BeforeAccess += BeforeAccessHandler;
            this.AfterAccess += AfterAccessHandler;
            Load();
        }
        private void Persist()
        {
            lock (_syncLock)
            {
                _cacheHelper.Set(_userId, Serialize());
                HasStateChanged = false;
            }
        }
        private void Load()
        {

            lock (_syncLock)
            {
                byte[] tokenCache = _cacheHelper.Get(_userId);
                Deserialize(tokenCache);
            }
        }
        private void AfterAccessHandler(TokenCacheNotificationArgs args)
        {
            if (this.HasStateChanged)
            {
                Persist();
            }
        }

        private void BeforeAccessHandler(TokenCacheNotificationArgs args)
        {
            Load();
        }
        public override void DeleteItem(TokenCacheItem item)
        {
            base.DeleteItem(item);
            Persist();
        }
        public override void Clear()
        {
            base.Clear();
            _cacheHelper.Remove(_userId);
        }
    }
}
