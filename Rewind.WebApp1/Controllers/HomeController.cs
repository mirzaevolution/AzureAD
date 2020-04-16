using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rewind.WebApp1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Rewind.WebApp1.Helpers;

namespace Rewind.WebApp1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string _userId;
        private readonly IDistributedCache _distributedCache;
        private readonly IOptions<AzureAdModel> _azureAdOptions;
        private readonly IOptions<TargetApiModel> _targetApiOptions;




        public HomeController(IDistributedCache distributedCache,
                IOptions<AzureAdModel> azureAdOptions,
                IOptions<TargetApiModel> targetApiOptions,
                ILogger<HomeController> logger)
        {
            _distributedCache = distributedCache;
            _azureAdOptions = azureAdOptions;
            _targetApiOptions = targetApiOptions;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("/AccessDenied")]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet("/Logout")]
        public IActionResult Logout()
        {
            string userObjectID = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            var authContext = new AuthenticationContext(_azureAdOptions.Value.Authority,
                                                        new DistributedMemoryTokenCache(userObjectID, _distributedCache));
            authContext.TokenCache.Clear();
            string redirectUrl = Url.Action(nameof(HomeController.Index), "Home", null, Request.Scheme);
            return SignOut(new AuthenticationProperties
            {
                AllowRefresh = true,
                RedirectUri = redirectUrl
            }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
