using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.Notifications;

[assembly: OwinStartup(typeof(AspNetMvc5.Startup))]

namespace AspNetMvc5
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                LoginPath = new PathString("/Home/Login"),
                LogoutPath = new PathString("/Home/Logout")
            })
                .UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
                {
                    Authority = ConfigurationManager.AppSettings["Authority"],
                    ClientId = ConfigurationManager.AppSettings["ClientId"],
                    ResponseType = "code id_token",
                    SaveTokens = true,
                    Scope = OpenIdConnectScope.OpenIdProfile,
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthenticationFailed = HandleAuthenticationFailed
                    },
                    PostLogoutRedirectUri = "https://localhost:44365/"
                });

        }

        private Task HandleAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            return context.Response.WriteAsync($"Some thing went wrong dude! {context.Exception.Message}");
        }
    }
}
