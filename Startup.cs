using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Vega.Core;
using Vega.Core.Domain;
using Vega.Core.Repositories.Helpers;
using Vega.Mapping;
using Vega.Persistence;

namespace Vega
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
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ISortHelper<Vehicle>, SortHelper<Vehicle>>();
            // Add Dbcontext
            services.AddDbContext<VegaDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Default"))
                    .LogTo(Console.WriteLine, LogLevel.Information));

            services.AddAutoMapper(typeof(MappingProfile));

            // // To list physical files from a path provided by configuration.
            // var physicalProvider = new PhysicalFileProvider(Configuration.GetValue<string>("StoredFilePath"));
            // services.AddSingleton<IFileProvider>(physicalProvider);

            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });

            // Register the Auth0 authentication service
            string domain = $"https://{Configuration["Auth0:Domain"]}";
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                // Register the JWT Bearer authentication scheme
                .AddJwtBearer(options =>
                {
                    options.Authority = domain;
                    options.Audience = Configuration["Auth0:Audience"];
                    // If the access token does not have a `sub` claim, `User.Identity.Name` will be `null`. Map it to a different claim by setting the NameClaimType below.
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                });
            
            // Add policies for the scopes.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("update:vehicles",
                    policy => policy.Requirements.Add(new HasScopeRequirement("update:vehicles", domain)));
            });
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            // Set the url to serve the vehicle photos file when we want to store those files in other folders other than wwwroot.
            app.UseStaticFiles(new StaticFileOptions
            {
                // Set the folder to store vehicle photos files.
                FileProvider = new PhysicalFileProvider(Configuration.GetValue<string>("StoredFilePath")),
                RequestPath = Configuration.GetValue<string>("RequestFilePath")
            });
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            // Add the Auth0 authentication and authorization middleware to the middleware pipeline
            app.UseAuthentication();
            app.UseAuthorization(); // This must locate between app.UseRouting and app.UseEndpoints to make middlewares function properly.

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}