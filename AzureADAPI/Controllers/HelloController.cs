using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AzureADAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class HelloController : ControllerBase
    {
        private static string _message = "World";

        private readonly IConfiguration _configuration;

        public HelloController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Access.Read")]
        [HttpGet("ReadMessage")]
        public IActionResult ReadMessage()
        {
            if (string.IsNullOrEmpty(_message))
            {
                _message = "World";
            }
            return Ok(new { message = $"Hello {_message}" });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Access.Write")]
        [HttpPost("WriteMessage")]
        public IActionResult WriteMessage([FromBody, Required]string message)
        {
            _message = message;
            return Ok();
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Access.Read")]
        [HttpGet("GetSecondaryApiMessage")]
        public async Task<IActionResult> GetSecondaryApiMessage()
        {
            try
            {
                ClientCredential clientCredential = new ClientCredential(_configuration["CurrentApiCreds:ClientId"], _configuration["CurrentApiCreds:ClientSecret"]);
                string accessToken = await HttpContext.GetTokenAsync("access_token");
                string assertionType = "urn:ietf:params:oauth:grant-type:jwt-bearer";
                string userName = HttpContext.User.FindFirst(ClaimTypes.Upn) != null
                            ? HttpContext.User.FindFirst(ClaimTypes.Upn).Value
                             : HttpContext.User.FindFirst(ClaimTypes.Email).Value;
                UserAssertion userAssertion = new UserAssertion(accessToken, assertionType, userName);
                AuthenticationContext authenticationContext = new AuthenticationContext(
                        _configuration["TargetApi:Authority"]);
                var result = await authenticationContext.AcquireTokenAsync(_configuration["TargetApi:Resource"], clientCredential, userAssertion);
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    var httpResponse = await client.GetAsync($"{_configuration["TargetApi:Address"]}/api/messages");
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        string content = await httpResponse.Content.ReadAsStringAsync();
                        return Ok(new { success = true, error = string.Empty, message = content });
                    }
                    return Ok(new { success = false, error = "Unauthorized", message = string.Empty });

                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, error = ex.Message, message = string.Empty });
            }
        }


    }
}