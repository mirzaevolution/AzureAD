
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Rewind.APILayer1.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Rewind.APILayer1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayerOneController : ControllerBase
    {
        private readonly IOptions<APISecurityCoreModel> _options;
        public LayerOneController(IOptions<APISecurityCoreModel> options)
        {
            _options = options;
        }

        [HttpGet("AccessReadTest")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Access.Read")]
        public string AccessReadTest()
        {
            return "You get read access!";
        }

        [HttpPost("AccessWriteTest")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Access.Write")]
        public string AccessWriteTest([FromBody]string data)
        {
            return $"You posted: {data}";
        }

        [HttpGet("GetLayerTwoMessage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Access.Read")]

        public async Task<string> GetLayerTwoMessage()
        {
            string accessTokenResult = await GetAccessToken();
            if (accessTokenResult.StartsWith("500"))
            {
                return accessTokenResult;
            }
            string accessToken = accessTokenResult.Split(':').LastOrDefault();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.BaseAddress = new Uri(_options.Value.TargetApi.Address);
                var response = await client.GetAsync("/api/LayerTwo/GetMessage");
                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    return responseText;
                }
                return $"{response.StatusCode}:{response.ReasonPhrase}";
            }
        }

        private async Task<string> GetAccessToken()
        {
            try
            {

                string accessToken = await HttpContext.GetTokenAsync("access_token");
                string assertionType = "urn:ietf:params:oauth:grant-type:jwt-bearer";
                string email = !string.IsNullOrEmpty(User.FindFirstValue(ClaimTypes.Upn)) ?
                    User.FindFirstValue(ClaimTypes.Upn) :
                    User.FindFirstValue(ClaimTypes.Email);
                ClientCredential clientCredential = new ClientCredential(_options.Value.CurrentApiCreds.ClientId, _options.Value.CurrentApiCreds.ClientSecret);
                UserAssertion userAssertion = new UserAssertion(accessToken, assertionType, email);
                AuthenticationContext context = new AuthenticationContext(_options.Value.TargetApi.Authority);
                var result = await context.AcquireTokenAsync(
                       _options.Value.TargetApi.Resource,
                       clientCredential, userAssertion
                   );
                return $"200:{result.AccessToken}";
            }
            catch (Exception ex)
            {
                return $"500:{ex.Message}";
            }
        }

    }
}