using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Client;
using System.Security.Claims;
using Microsoft.Extensions.Caching;
using AzureMSALWebApp.Models;
using Microsoft.AspNetCore.Http;
using static IdentityModel.OidcConstants;
using AzureMSALWebApp.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using IdentityModel;

namespace AzureMSALWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _azOptions = new AzureAdModel();

        }

        public IConfiguration Configuration { get; }
        private AzureAdModel _azOptions;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _azOptions = new AzureAdModel();
            var section = Configuration.GetSection("AzureAd");
            section.Bind(_azOptions);
            services.AddOptions();
            services.Configure<AzureAdModel>(section);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.AccessDeniedPath = new PathString("/AccessDenied");
                    options.LoginPath = new PathString("/Login");
                    options.LogoutPath = new PathString("/Logout");
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = _azOptions.Authority;
                    options.ClientId = _azOptions.ClientId;
                    options.ClientSecret = _azOptions.ClientSecret;
                    options.AccessDeniedPath = new PathString("/AccessDenied");
                    options.SaveTokens = true;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.ResponseType = ResponseTypes.CodeIdToken;
                    options.CallbackPath = _azOptions.CallbackPath;
                    options.Events.OnAuthorizationCodeReceived += OnAuthorizationCodeReceived;
                    options.TokenValidationParameters.ValidateIssuer = false;
                });
            services.AddDistributedMemoryCache();
            services.AddControllersWithViews();
        }

        private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            string authority = context.Options.Authority;
            string clientId = context.Options.ClientId;
            string clientSecret = context.Options.ClientSecret;
            string redirectUri = context.TokenEndpointRequest.RedirectUri;
            string key = context.Principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
            string code = context.TokenEndpointRequest.Code;
            //IEnumerable<string> scopes = _azOptions.Scopes.Split(";").Where(c => !string.IsNullOrEmpty(c));
            IEnumerable<string> scopes = new string[] { "api://core/.default" };
            IDistributedCache cache = context.HttpContext.RequestServices.GetService<IDistributedCache>();

            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(authority)
                .WithRedirectUri(redirectUri)
                .Build();
            TokenCacheHelper.Initialize(key: key,
                    distributedCache: cache,
                    tokenCache: app.UserTokenCache);

            var result = await app.AcquireTokenByAuthorizationCode(scopes, code)
              .ExecuteAsync();
            context.HandleCodeRedemption(result.AccessToken, result.IdToken);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
