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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using AzureMSALAPIOne.Models;
using AzureMSALAPIOne.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace AzureMSALAPIOne
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
            JwtBearerAuthModel jwtModel = new JwtBearerAuthModel();
            Configuration.GetSection("JwtBearer").Bind(jwtModel);
            services.AddOptions();
            services.Configure<JwtBearerAuthModel>(Configuration.GetSection("JwtBearer"));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = jwtModel.Authority;
                    options.Audience = jwtModel.Audience;
                    options.SaveToken = true;
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Access.Read", o =>
                 {
                     o.RequireAuthenticatedUser();
                     o.Requirements.Add(new ReadAccessScopeRequirement());
                 });
            });
            services.AddMemoryCache();

            services.AddTransient<IAuthorizationHandler, ReadAcccessScopeHandler>();
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
