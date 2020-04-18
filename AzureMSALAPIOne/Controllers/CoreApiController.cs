using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using AzureMSALAPIOne.Models;
using Microsoft.AspNetCore.Authentication;
using IdentityModel;
using System.Security.Claims;

namespace AzureMSALAPIOne.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoreApiController : ControllerBase
    {
        private readonly IOptions<JwtBearerAuthModel> _options;
        private readonly IMemoryCache _memoryCache;
        private static object _syncLock = new object();

        public CoreApiController(
            IMemoryCache memoryCache,
            IOptions<JwtBearerAuthModel> options)
        {
            _memoryCache = memoryCache;
            _options = options;
        }

        [Authorize(Policy = "Access.Read")]
        [HttpGet(nameof(GetMessage))]
        public string GetMessage()
        {
            return "You get message from endpoint 1!";
        }
        [Authorize(Roles = "Reader")]
        [HttpGet(nameof(GetMessageWithRole))]
        public string GetMessageWithRole()
        {
            return "You get message from endpoint 1!";
        }

        [Authorize(Policy = "Access.Read")]
        [HttpGet(nameof(GetMessageFrom2ndApi))]
        public async Task<string> GetMessageFrom2ndApi()
        {
            string token = await GetAccessToken();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_options.Value.TargetApiBaseAddress);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("/api/Messages");
                if (response.IsSuccessStatusCode)
                {
                    string text = await response.Content.ReadAsStringAsync();
                    return text;
                }
                return response.StatusCode.ToString();
            }
        }
        private async Task<string> GetAccessToken()
        {
            var app = GetConfidentialClientApplication();
            var scopes = _options.Value.TargetApiScopes.Split(';');
            var account = (await app.GetAccountsAsync()).FirstOrDefault();
            Microsoft.Identity.Client.AuthenticationResult authResult = null;
            try
            {
                authResult = await app.AcquireTokenSilent(scopes, account).ExecuteAsync();
                return authResult.AccessToken;
            }
            catch (MsalUiRequiredException ex)
            {
                string token = await HttpContext.GetTokenAsync(OidcConstants.TokenTypes.AccessToken);
                UserAssertion userAssertion = new UserAssertion(token, OidcConstants.GrantTypes.JwtBearer);

                authResult = await app.AcquireTokenOnBehalfOf(scopes, userAssertion).ExecuteAsync();
                return authResult.AccessToken;
            }
        }
        private IConfidentialClientApplication GetConfidentialClientApplication()
        {
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                .Create(_options.Value.ClientId)
                .WithClientSecret(_options.Value.ClientSecret)
                .WithAuthority(_options.Value.Authority)
                .WithDebugLoggingCallback(LogLevel.Verbose)
                .Build();
            app.UserTokenCache.SetBeforeAccessAsync(OnBeforeCacheHandler);
            app.UserTokenCache.SetAfterAccessAsync(OnAfterCacheHandler);

            return app;
        }
        private Task OnAfterCacheHandler(TokenCacheNotificationArgs context)
        {
            if (context.HasStateChanged)
            {
                lock (_syncLock)
                {

                    string usernameOrEmail = string.Empty;
                    if (context.Account != null)
                    {
                        usernameOrEmail = context.Account.Username;
                    }
                    else
                    {
                        usernameOrEmail = User.FindFirstValue(JwtClaimTypes.Email);
                        if (string.IsNullOrEmpty(usernameOrEmail))
                        {
                            usernameOrEmail = User.FindFirstValue(ClaimTypes.Email);
                        }
                    }
                    _memoryCache.Set(usernameOrEmail, context.TokenCache.SerializeMsalV3());
                }
            }
            return Task.CompletedTask;
        }

        private Task OnBeforeCacheHandler(TokenCacheNotificationArgs context)
        {
            string usernameOrEmail = string.Empty;
            if (context.Account != null)
            {
                usernameOrEmail = context.Account.Username;
            }
            else
            {
                usernameOrEmail = User.FindFirstValue(JwtClaimTypes.Email);
                if (string.IsNullOrEmpty(usernameOrEmail))
                {
                    usernameOrEmail = User.FindFirstValue(ClaimTypes.Email);
                }
            }
            if (_memoryCache.TryGetValue(usernameOrEmail, out byte[] data))
            {
                context.TokenCache.DeserializeMsalV3(data);
            }
            else
            {
                context.TokenCache.DeserializeMsalV3(null);
            }
            return Task.CompletedTask;
        }
    }
}