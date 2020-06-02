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
using Microsoft.AspNetCore.Http;
using IdentityModel;

namespace SkylineWebApp
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
                    options.Authority = Configuration["AzureAD:Authority"];
                    options.ClientId = Configuration["AzureAD:ClientId"];
                    options.ClientSecret = Configuration["AzureAD:ClientSecret"];
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.ResponseType = OidcConstants.ResponseTypes.CodeIdToken;
                    options.SaveTokens = true;
                    options.Events.OnAuthorizationCodeReceived += OnAuthorizationCodeReceived;
                    options.TokenValidationParameters.ValidateIssuer = false;
                });
            services.AddControllersWithViews();
        }

        private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            if (!context.HandledCodeRedemption)
            {
                IConfidentialClientApplication confidentialClient =
                    ConfidentialClientApplicationBuilder.Create(Configuration["AzureAD:ClientId"])
                    .WithClientSecret(Configuration["AzureAD:ClientSecret"])
                    .WithAuthority(Configuration["AzureAD:Authority"])
                    .WithRedirectUri(context.TokenEndpointRequest.RedirectUri)
                    .Build();

                string scope = Configuration["AzureAD:Scopes"];
                string code = context.TokenEndpointRequest.Code;

                var result = await confidentialClient.AcquireTokenByAuthorizationCode(new string[] { scope }, code)
                    .ExecuteAsync();
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
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                .RequireAuthorization();
            });
        }
    }
}
