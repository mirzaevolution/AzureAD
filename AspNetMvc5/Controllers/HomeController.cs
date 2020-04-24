using AspNetMvc5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Cookies;

namespace AspNetMvc5.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult SecurityClaims()
        {
            List<SecurityDataClaim> list = new List<SecurityDataClaim>();
            foreach (var item in HttpContext.GetOwinContext().Authentication.User.Claims)
            {
                list.Add(new SecurityDataClaim
                {
                    Key = item.Type,
                    Data = item.Value
                });
            }
            return View(list);
        }
        [AllowAnonymous]
        public void Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(new AuthenticationProperties
            {
                RedirectUri = "https://localhost:44365/"
            }, OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }
    }
}