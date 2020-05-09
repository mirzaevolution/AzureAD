using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Rewind.WPFClient
{
    public class GenericFileTokenCache : TokenCache
    {
        private static object _syncLock = new object();
        private static string _fileLocation = "cache-v1.dat";
        public GenericFileTokenCache()
        {
            this.BeforeAccess += BeforeAccessHandler;
            this.AfterAccess += AfterAccessHandler;
        }

        private void Load()
        {
            DeserializeAdalV3(File.Exists(_fileLocation) ? File.ReadAllBytes(_fileLocation) : null);
        }
        private void Persist()
        {
            lock (_syncLock)
            {
                File.WriteAllBytes(_fileLocation, SerializeAdalV3());
                this.HasStateChanged = false;
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
    }
}
