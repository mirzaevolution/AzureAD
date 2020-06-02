using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Cache;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DeviceCodeFlowsSampleV1
{
    class Program
    {
        private static readonly string _authority = "https://login.microsoftonline.com/mscevo.onmicrosoft.com";
        private static readonly string _clientId = "a256d239-94d3-4e20-a667-e106e2ee9f98";
        private static readonly string _redirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient";
        private static readonly string[] _scopes = { "api://core/.default" };
        private static readonly string _targetApiAddress = "https://localhost:44318/api/CoreApi/GetMessage";
        private static object _syncLock = new object();
        private static readonly string _cacheLocation = "cache.bin";
        private static IPublicClientApplication InitApp()
        {
            IPublicClientApplication app =
                PublicClientApplicationBuilder.Create(_clientId)
                .WithRedirectUri(_redirectUri)
                .WithAuthority(_authority)
                .Build();
            app.UserTokenCache.SetBeforeAccessAsync(OnBeforeAccess);
            app.UserTokenCache.SetAfterAccessAsync(OnAfterAccess);
            return app;
        }

        private static Task OnAfterAccess(TokenCacheNotificationArgs context)
        {
            if (context.HasStateChanged)
            {
                lock (_syncLock)
                {
                    File.WriteAllBytesAsync(_cacheLocation, context.TokenCache.SerializeMsalV3());
                }
            }
            return Task.CompletedTask;
        }

        private static async Task OnBeforeAccess(TokenCacheNotificationArgs context)
        {
            byte[] rawData = File.Exists(_cacheLocation) ? await File.ReadAllBytesAsync(_cacheLocation) : null;
            context.TokenCache.DeserializeMsalV3(rawData);
        }

        private static async Task<string> InitAuth()
        {
            var app = InitApp();
            var accounts = await app.GetAccountsAsync();
            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
                return result.AccessToken;
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    result = await app.AcquireTokenWithDeviceCode(_scopes, r =>
                    {
                        Console.WriteLine(r.Message);
                        return Task.CompletedTask;
                    }).ExecuteAsync();
                    return result.AccessToken;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[DEV-FLOW-ERROR]");
                    Console.WriteLine(ex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR]");
                Console.WriteLine(ex.ToString());
            }
            return "401";
        }

        private static async Task CallApi()
        {
            try
            {
                string token = await InitAuth();
                if (token != "401")
                {
                    using (HttpClient client = new HttpClient())
                    {
                        Console.WriteLine("Calling api...");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                        var response = await client.GetAsync(_targetApiAddress);
                        if (response.IsSuccessStatusCode)
                        {
                            string message = await response.Content.ReadAsStringAsync();
                            Console.WriteLine(message);
                        }
                        else
                        {
                            Console.WriteLine(response.StatusCode.ToString());
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Access denied");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void Main(string[] args)
        {
            CallApi().Wait();
            Console.ReadLine();
        }
    }
}
