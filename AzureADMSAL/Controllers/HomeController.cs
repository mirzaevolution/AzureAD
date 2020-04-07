using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AzureADMSAL.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using AzureADMSAL.Helpers;

namespace AzureADMSAL.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IOptions<AzureActiveDirectoryOptions> _options;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _distributedCache;
        public HomeController(
            IConfiguration configuration,
            IOptions<AzureActiveDirectoryOptions> options,
            IDistributedCache distributedCache
            )
        {
            _options = options;
            _configuration = configuration;
            _distributedCache = distributedCache;
        }
        public IActionResult Index()
        {
            List<Claim> allClaims = User.Claims.ToList();
            return View(allClaims);
        }
        [Route("/AccessDenied")]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SignOut()
        {
            // Remove all cache entries for this user and send an OpenID Connect sign-out request.
            string userObjectID = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            var authContext = new AuthenticationContext(_options.Value.Instance + _options.Value.TenantId,
                                                        new DistributedMemoryTokenCache(userObjectID, _distributedCache));
            authContext.TokenCache.Clear();

            // Let Azure AD sign-out
            var callbackUrl = Url.Action(nameof(SignedOut), "Home", values: null, protocol: Request.Scheme);
            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl, AllowRefresh = true },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult SignedOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
