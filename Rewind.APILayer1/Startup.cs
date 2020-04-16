using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.AspNetCore.Http;
using Rewind.APILayer1.Models;
using Rewind.APILayer1.Helpers;

namespace Rewind.APILayer1
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
            APISecurityCoreModel apiSecurityModel = new APISecurityCoreModel();
            Configuration.GetSection("APISecurityCoreModel").Bind(apiSecurityModel);
            services.AddOptions();
            var section = Configuration.GetSection("APISecurityCoreModel");
            services.Configure<APISecurityCoreModel>(section);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = apiSecurityModel.JwtBearer.Authority;
                    options.Audience = apiSecurityModel.JwtBearer.Audience;
                    options.SaveToken = true;
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Access.Read", o =>
                {
                    o.RequireAuthenticatedUser();
                    o.Requirements.Add(new ReadAccessRequirement());
                });
                options.AddPolicy("Access.Write", o =>
                {
                    o.RequireAuthenticatedUser();
                    o.Requirements.Add(new WriteAccessRequirement());
                });
            });
            services.AddSingleton<IAuthorizationHandler, ReadAccessAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, WriteAccessAuthorizationHandler>();

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
