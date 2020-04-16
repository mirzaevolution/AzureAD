using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AzureADMSAL.Helpers;
using AzureADMSAL.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace AzureADMSAL.Controllers
{
    [Authorize]
    public class ApiHandlerController : Controller
    {
        private readonly IOptions<AzureActiveDirectoryOptions> _options;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _distributedCache;
        public ApiHandlerController(
            IConfiguration configuration,
            IOptions<AzureActiveDirectoryOptions> options,
            IDistributedCache distributedCache
            )
        {
            _options = options;
            _configuration = configuration;
            _distributedCache = distributedCache;
        }
        public async Task<IActionResult> CallApi()
        {
            string token = await GetAccessToken();
            if (token == "401")
            {
                return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
            }
            using (HttpClient client = new HttpClient())
            {
                string baseAddress = _configuration["ApiEndpoint"];
                string url = $"{baseAddress}/Api/hello/ReadMessage";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage responseMessage = await client.GetAsync(url);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string jsonResponse = await responseMessage.Content.ReadAsStringAsync();
                    HelloApiResponse helloApiResponse = JsonConvert.DeserializeObject<HelloApiResponse>(jsonResponse);
                    ViewBag.Message = helloApiResponse.Message;
                }
                else
                {
                    ViewBag.Message = "Unauthorized";
                }
            }
            return View();
        }
        public async Task<IActionResult> CallMessageApi()
        {
            string token = await GetAccessToken();
            if (token == "401")
            {
                return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
            }
            using (HttpClient client = new HttpClient())
            {
                string baseAddress = _configuration["ApiEndpoint"];
                string url = $"{baseAddress}/Api/hello/GetSecondaryApiMessage";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage responseMessage = await client.GetAsync(url);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string jsonResponse = await responseMessage.Content.ReadAsStringAsync();
                    HelloApiResponse helloApiResponse = JsonConvert.DeserializeObject<HelloApiResponse>(jsonResponse);
                    ViewBag.Message = helloApiResponse.Message;
                }
                else
                {
                    ViewBag.Message = "Unauthorized";
                }
            }
            return View();
        }
        private async Task<string> GetAccessToken()
        {
            try
            {
                string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;
                DistributedMemoryTokenCache distributedMemoryTokenCache = new DistributedMemoryTokenCache(userObjectID, _distributedCache);
                AuthenticationContext authenticationContext = new AuthenticationContext(
                        $"{_options.Value.Instance}{_options.Value.TenantId}", distributedMemoryTokenCache
                    );

                var result = await authenticationContext
                    .AcquireTokenSilentAsync(
                        _options.Value.Resource,
                        new ClientCredential(_options.Value.ClientId, _options.Value.ClientSecret),
                        new UserIdentifier(userObjectID, UserIdentifierType.UniqueId)
                        );
                return result.AccessToken;
            }
            catch (Exception ex)
            {
                return "401";
            }
        }
    }
}