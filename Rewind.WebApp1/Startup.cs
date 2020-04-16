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
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.AspNetCore.Http;
using IdentityModel;
using Rewind.WebApp1.Models;
using Rewind.WebApp1.Helpers;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;

namespace Rewind.WebApp1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AzureAdModel azureAdModel = new AzureAdModel();
            Configuration.Bind("AzureActiveDirectory", azureAdModel);
            services.AddOptions();
            IConfigurationSection azureAdOption = Configuration.GetSection("AzureActiveDirectory");
            IConfigurationSection targetApiOption = Configuration.GetSection("TargetApi");
            services.Configure<AzureAdModel>(azureAdOption);
            services.Configure<TargetApiModel>(targetApiOption);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.AccessDeniedPath = new PathString("/AccessDenied");
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = azureAdModel.Authority;
                    options.ClientId = azureAdModel.ClientId;
                    options.ClientSecret = azureAdModel.ClientSecret;
                    options.CallbackPath = azureAdModel.CallbackPath;
                    options.Resource = azureAdModel.Resource;
                    options.ResponseType = OidcConstants.ResponseTypes.CodeIdToken;
                    options.SaveTokens = true;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Events.OnAuthorizationCodeReceived += HandleApiToken;
                });
            services.AddDistributedMemoryCache();
            services.AddControllersWithViews();
        }

        private async Task HandleApiToken(AuthorizationCodeReceivedContext context)
        {
            if (!context.HandledCodeRedemption)
            {

                string userId = context.Principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
                IDistributedCache distributedCache = context.HttpContext.RequestServices.GetService<IDistributedCache>();
                DistributedMemoryTokenCache tokenCache = new DistributedMemoryTokenCache(userId, distributedCache);
                string authority = context.Options.Authority;
                string code = context.TokenEndpointRequest.Code;
                string resource = context.Options.Resource;
                string clientId = context.Options.ClientId;
                string clientSecret = context.Options.ClientSecret;
                string redirectUri = context.TokenEndpointRequest.RedirectUri;
                AuthenticationContext authenticationContext = new AuthenticationContext(authority, true, tokenCache);
                var result = await authenticationContext.AcquireTokenByAuthorizationCodeAsync(code, new Uri(redirectUri), new ClientCredential(clientId, clientSecret), resource);
                context.HandleCodeRedemption(result.AccessToken, result.IdToken);
            }
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
