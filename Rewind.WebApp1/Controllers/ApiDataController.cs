using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Rewind.WebApp1.Helpers;
using Rewind.WebApp1.Models;

namespace Rewind.WebApp1.Controllers
{
    [Authorize]

    public class ApiDataController : Controller
    {

        private string _userId;
        private readonly IDistributedCache _distributedCache;
        private readonly IOptions<AzureAdModel> _azureAdOptions;
        private readonly IOptions<TargetApiModel> _targetApiOptions;


        public ApiDataController(
                IDistributedCache distributedCache,
                IOptions<AzureAdModel> azureAdOptions,
                IOptions<TargetApiModel> targetApiOptions
            )
        {
            _distributedCache = distributedCache;
            _azureAdOptions = azureAdOptions;
            _targetApiOptions = targetApiOptions;
        }

        public async Task<IActionResult> FirstApi()
        {
            var tokenResult = await GetAccessToken();
            if (tokenResult.StartsWith("500"))
            {
                return View((object)tokenResult);
            }
            else if (tokenResult.StartsWith("401"))
            {
                return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
            }
            string token = tokenResult.Split(':').LastOrDefault();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_targetApiOptions.Value.Address);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var httpResponse = await client.GetAsync("/api/LayerOne/AccessReadTest");
                if (httpResponse.IsSuccessStatusCode)
                {
                    string responseText = await httpResponse.Content.ReadAsStringAsync();
                    return View((object)responseText);
                }
                return View((object)httpResponse.StatusCode);
            }
        }
        public async Task<IActionResult> SecondaryApi()
        {
            var tokenResult = await GetAccessToken();
            if (tokenResult.StartsWith("500"))
            {
                return View((object)tokenResult);
            }
            else if (tokenResult.StartsWith("401"))
            {
                return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
            }
            string token = tokenResult.Split(':').LastOrDefault();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_targetApiOptions.Value.Address);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var httpResponse = await client.GetAsync("/api/LayerOne/GetLayerTwoMessage");
                if (httpResponse.IsSuccessStatusCode)
                {
                    string responseText = await httpResponse.Content.ReadAsStringAsync();
                    return View((object)responseText);
                }
                return View((object)httpResponse.StatusCode);
            }
        }

        private async Task<string> GetAccessToken()
        {
            try
            {
                _userId = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");

                AuthenticationContext authenticationContext = new AuthenticationContext(
                        _azureAdOptions.Value.Authority,
                        true,
                        new DistributedMemoryTokenCache(_userId, _distributedCache)
                    );

                var result = await authenticationContext.AcquireTokenSilentAsync(
                        _azureAdOptions.Value.Resource,
                        new ClientCredential(_azureAdOptions.Value.ClientId, _azureAdOptions.Value.ClientSecret),
                        new UserIdentifier(_userId, UserIdentifierType.UniqueId)
                    );
                return $"200:{result.AccessToken}";
            }
            catch (AdalException ex)
            {
                return $"401:{ex.Message}";
            }
            catch (Exception ex)
            {
                return $"500:{ex.Message}";
            }
        }
    }
}