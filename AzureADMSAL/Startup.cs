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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Authorization;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using AzureADMSAL.Helpers;
using IdentityModel.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using AzureADMSAL.Models;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;

namespace AzureADMSAL
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
            AzureActiveDirectoryOptions aadOptions = new AzureActiveDirectoryOptions();
            Configuration.Bind("AzureActiveDirectory", aadOptions);

            services.AddOptions();
            var section = Configuration.GetSection("AzureActiveDirectory");
            services.Configure<AzureActiveDirectoryOptions>(section);
            //services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
            //    .AddAzureAD(options => Configuration.Bind("AzureActiveDirectory", options));
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;

            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                 {
                     options.AccessDeniedPath = new PathString("/AccessDenied");
                 })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = $"{aadOptions.Instance}{aadOptions.TenantId}";
                    options.ClientId = aadOptions.ClientId;
                    options.ClientSecret = aadOptions.ClientSecret;
                    options.ResponseType = OidcConstants.ResponseTypes.CodeIdToken;
                    options.AccessDeniedPath = new PathString("/AccessDenied");
                    options.UseTokenLifetime = true;
                    options.CallbackPath = aadOptions.CallbackPath;
                    options.Resource = aadOptions.Resource;
                    options.Events.OnAuthorizationCodeReceived += HandleAuthorizationCode;
                });
            services.AddDistributedMemoryCache();

            services.AddControllersWithViews();
        }

        private async Task HandleAuthorizationCode(AuthorizationCodeReceivedContext context)
        {
            if (!context.HandledCodeRedemption)
            {
                string userObjectId = context.Principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
                ClientCredential clientCredential = new ClientCredential(context.Options.ClientId, context.Options.ClientSecret);
                IDistributedCache distributedCache = context.HttpContext.RequestServices.GetService<IDistributedCache>();
                DistributedMemoryTokenCache distributedMemoryTokenCache = new DistributedMemoryTokenCache(userObjectId, distributedCache);
                AuthenticationContext authenticationContext = new AuthenticationContext(context.Options.Authority, distributedMemoryTokenCache);
                string code = context.TokenEndpointRequest.Code;
                var authenticateResult = await authenticationContext.AcquireTokenByAuthorizationCodeAsync(code, new Uri(context.TokenEndpointRequest.RedirectUri), clientCredential, context.Options.Resource);
                context.HandleCodeRedemption(authenticateResult.AccessToken, authenticateResult.IdToken);
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
