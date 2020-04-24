using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AzureMSALWebApp.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using IdentityModel;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AzureMSALWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOptions<AzureAdModel> _options;
        public HomeController(
            IOptions<AzureAdModel> options,
            ILogger<HomeController> logger)
        {
            _logger = logger;
            _options = options;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("/Login")]
        public IActionResult Login()
        {
            AuthenticationProperties props = new AuthenticationProperties();
            props.RedirectUri = Url.Action("Index", "Home", null, Request.Scheme);

            return Challenge(props, OpenIdConnectDefaults.AuthenticationScheme);
        }
        [Route("/Logout")]
        [Authorize]
        public IActionResult Logout()
        {
            string redirectUrl = Url.Action("Index", "Home", null, Request.Scheme);
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
