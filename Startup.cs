using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Vega.Controllers;
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
            // Because IPhotoService is only being used when we send request to UploadPhotos method
            // and it is a no state object
            // so we can just use AddTransient to add it into DI.
            services.AddTransient<IPhotoService, PhotoService>();
            services.AddTransient<IPhotoStorage, FileSystemPhotoStorage>();
            // Add Dbcontext and log the operation into the console.
            services.AddDbContext<VegaDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Default"))
                    .LogTo(Console.WriteLine, LogLevel.Information));

            services.AddAutoMapper(typeof(MappingProfile));

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

            services.AddAuthorization(options =>
            {
                // Add policies for the claims.
                options.AddPolicy(Policies.RequireAdminRole,
                    policy => policy.RequireClaim("https://vega.com/role", Policies.RequireAdminRole));
                // options.AddPolicy("update:vehicles",
                //     policy => policy.Requirements.Add(new HasScopeRequirement("update:vehicles", domain)));
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
                    // We start the angular app through the docker now so we don't use the spa.UseAngularCliServer
                    // method to start the angular app now. We just use spa.UseProxyToSpaDevelopmentServer to send the
                    // request to the spa app.
                    spa.UseProxyToSpaDevelopmentServer("http://spa:4200");
                }
            });
        }
    }
}