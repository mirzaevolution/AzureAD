using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkylineWebApi.Models;
using SkylineWebApi.Helpers;

namespace SkylineWebApi
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
            services.AddOptions<AuthorizationRule>();

            var authorizationRule = new AuthorizationRule();
            Configuration.Bind("Authorization", authorizationRule);

            services.Configure<AuthorizationRule>(Configuration.GetSection("Authorization"));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = Configuration["AzureAD:Authority"];
                    options.Audience = Configuration["AzureAD:Audience"];
                    options.SaveToken = true;
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ClaimAuthorization", policy =>
                {
                    policy.Requirements.Add(new CountryRequirement(authorizationRule));
                    policy.Requirements.Add(new GroupRequirement(authorizationRule));
                });
            });
            services.AddTransient<IAuthorizationHandler, CountryAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, GroupAuthorizationHandler>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
