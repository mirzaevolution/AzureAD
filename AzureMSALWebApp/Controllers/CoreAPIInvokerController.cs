using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using AzureMSALWebApp.Models;
using AzureMSALWebApp.Helpers;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using IdentityModel;

namespace AzureMSALWebApp.Controllers
{
    [Authorize]
    public class CoreAPIInvokerController : Controller
    {
        private readonly IDistributedCache _cache;
        private readonly IOptions<AzureAdModel> _options;
        public CoreAPIInvokerController(
                IDistributedCache cache,
                IOptions<AzureAdModel> options
            )
        {
            _cache = cache;
            _options = options;

        }

        public async Task<IActionResult> SingleHopApi()
        {
            string accessTokenResponse = await GetAccessToken();
            string accessTokenCode = accessTokenResponse.Split(':').FirstOrDefault();
            switch (accessTokenCode)
            {
                case "200":
                    string response = await CallApi(
                            baseAddress: _options.Value.TargetApiBaseAddress,
                            pathAddress: "/api/CoreApi/GetMessage",
                            accessToken: accessTokenResponse.Split(':').LastOrDefault()?.Trim()
                        );
                    return View((object)response);
                case "401":
                    AuthenticationProperties properties = new AuthenticationProperties();
                    ICollection<string> scopes = new string[]
                    {
                        OidcConstants.StandardScopes.OpenId,
                        OidcConstants.StandardScopes.OfflineAccess,
                        OidcConstants.StandardScopes.Profile
                    }.Union(_options.Value.Scopes.Split(':')).ToList();

                    properties.SetParameter<ICollection<string>>(OpenIdConnectParameterNames.Scope, scopes);
                    return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
                case "500":
                    return View((object)accessTokenResponse);
            }
            return View((object)"Unknown response");
        }
        public async Task<IActionResult> MultiHopApi()
        {

            string accessTokenResponse = await GetAccessToken();
            string accessTokenCode = accessTokenResponse.Split(':').FirstOrDefault();
            switch (accessTokenCode)
            {
                case "200":
                    string response = await CallApi(
                            baseAddress: _options.Value.TargetApiBaseAddress,
                            pathAddress: "/api/CoreApi/GetMessageFrom2ndApi",
                            accessToken: accessTokenResponse.Split(':').LastOrDefault()?.Trim()
                        );
                    return View((object)response);
                case "401":
                    AuthenticationProperties properties = new AuthenticationProperties();
                    ICollection<string> scopes = new string[]
                    {
                        OidcConstants.StandardScopes.OpenId,
                        OidcConstants.StandardScopes.OfflineAccess,
                        OidcConstants.StandardScopes.Profile
                    }.Union(_options.Value.Scopes.Split(':')).ToList();

                    properties.SetParameter<ICollection<string>>(OpenIdConnectParameterNames.Scope, scopes);
                    return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
                case "500":
                    return View((object)accessTokenResponse);
            }
            return View((object)"Unknown response");
        }

        #region Helpers
        private async Task<string> GetAccessToken()
        {
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                .Create(_options.Value.ClientId)
                .WithClientSecret(_options.Value.ClientSecret)
                .WithAuthority(_options.Value.Authority)
                .Build();

            string key = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
            TokenCacheHelper.Initialize(
                    key,
                    _cache,
                    app.UserTokenCache
                );
            IEnumerable<string> scopes = new string[]
            {
                OidcConstants.StandardScopes.OpenId,
                OidcConstants.StandardScopes.Profile,
                OidcConstants.StandardScopes.OfflineAccess
            }.Union(_options.Value.Scopes.Split(';'));
            try
            {
                var account = (await app.GetAccountsAsync()).FirstOrDefault();
                var result = await app.AcquireTokenSilent(scopes, account)
                    .ExecuteAsync();
                return $"200:{result.AccessToken}";
            }
            catch (MsalUiRequiredException)
            {
                return $"401:unauthorized";
            }
            catch (Exception ex)
            {
                return $"500:{ex.Message}";
            }
        }
        private async Task<string> CallApi(string baseAddress, string pathAddress, string accessToken)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage response =
                    await client.GetAsync(pathAddress);
                if (response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();
                    return responseText;
                }
                else
                {
                    return $"500:{response.ReasonPhrase}";
                }
            }
        }
        #endregion
    }
}