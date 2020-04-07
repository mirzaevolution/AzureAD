using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;

namespace AzureADMSAL.Helpers
{
    public class OpenIdConnectOAuthHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private string _baseAddress = "";
        public OpenIdConnectOAuthHelper(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _baseAddress = $"{_configuration["AzureActiveDirectory:Instance"]}{_configuration["AzureActiveDirectory:TenantId"]}";
        }
        public async Task<string> DetectAndGetToken()
        {
            string token = string.Empty;
            string expiresAtString = await _httpContextAccessor.HttpContext.GetTokenAsync("expires_at");

            if (!string.IsNullOrEmpty(expiresAtString))
            {
                HttpClient client = _httpClientFactory.CreateClient();
                string currentAccessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                if ((DateTimeOffset.Parse(expiresAtString) < DateTimeOffset.UtcNow))
                {
                    token = await RenewToken(client, $"{_baseAddress}/oauth2/v2.0/token");
                }
                else
                {
                    token = currentAccessToken;
                }
            }

            return token;
        }
        private async Task<string> RenewToken(HttpClient client, string tokenEndpoint)
        {

            string token = string.Empty;
            string refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(
                    OpenIdConnectParameterNames.RefreshToken
                );

            TokenResponse tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = tokenEndpoint,
                ClientId = _configuration["AzureActiveDirectory:ClientId"],
                ClientSecret = _configuration["AzureActiveDirectory:ClientSecret"],
                GrantType = OpenIdConnectGrantTypes.RefreshToken,
                RefreshToken = refreshToken,
            });
            if (tokenResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                token = tokenResponse.AccessToken;
                DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
                List<AuthenticationToken> authenticationTokens = new List<AuthenticationToken>
                                {
                                    new AuthenticationToken { Name = OpenIdConnectParameterNames.IdToken, Value = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken) },
                                    new AuthenticationToken { Name = OpenIdConnectParameterNames.AccessToken, Value = tokenResponse.AccessToken },
                                    new AuthenticationToken { Name = OpenIdConnectParameterNames.RefreshToken, Value = tokenResponse.RefreshToken},
                                    new AuthenticationToken { Name = "expires_at", Value = expiresAt.ToString("o") }
                                };
                var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();
                if (authenticationResult.Succeeded)
                {
                    authenticationResult.Properties.StoreTokens(authenticationTokens);
                    await _httpContextAccessor
                        .HttpContext
                        .SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            authenticationResult.Principal,
                            authenticationResult.Properties);
                }
                return token;
            }


            return token;
        }

    }
}
