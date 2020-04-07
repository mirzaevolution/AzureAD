using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.IO;
using System.Security.Cryptography;

namespace AdalNetWpf
{
    public class FileCache : TokenCache
    {
        public string FileLocation { get; set; }
        private static object _lockState = new object();

        public FileCache(string fileLocation = "cache.dat")
        {
            FileLocation = fileLocation;
            InitializeComponents();
        }
        private void InitializeComponents()
        {
            this.AfterAccess = AfterAccessHandler;
            this.BeforeAccess = BeforeAccessHandler;
        }
        private void BeforeAccessHandler(TokenCacheNotificationArgs args)
        {
            lock (_lockState)
            {
                DeserializeAdalV3(File.Exists(FileLocation) ?
                        ProtectedData.Unprotect(File.ReadAllBytes(FileLocation), null, DataProtectionScope.CurrentUser) :
                        null);
            }
        }

        private void AfterAccessHandler(TokenCacheNotificationArgs args)
        {
            if (this.HasStateChanged)
            {
                File.WriteAllBytes(FileLocation,
                            ProtectedData.Protect(SerializeAdalV3(), null, DataProtectionScope.CurrentUser)
                        );
                this.HasStateChanged = false;
            }
        }
    }
}
