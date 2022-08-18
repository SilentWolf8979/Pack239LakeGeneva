using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Pack239LakeGeneva.Data;
using Pack239LakeGeneva.Models;
using Pack239LakeGeneva.Services;

using System;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Pack239LakeGeneva
{
  public class Startup
  {
    private IConfiguration Configuration { get; }
    private IWebHostEnvironment _env { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
      Configuration = configuration;
      _env = environment;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddHealthChecks();
      
      // Can be removed once UseMvc is updated to use endpoint routing
      services.AddMvc(options => options.EnableEndpointRouting = false);

      if (!_env.IsDevelopment())//(_env.IsStaging() || _env.IsProduction())
      {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("AzureConnection")));
      }
      else
      {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("LocalConnection")));
      }

      services.AddIdentity<ApplicationUser, IdentityRole>()
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();

      services.ConfigureApplicationCookie(options =>
      {
        options.Events.OnRedirectToLogin = context =>
        {
          context.Response.StatusCode = 401;
          return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
          context.Response.StatusCode = 403;
          return Task.CompletedTask;
        };
      });

      // Add application services.
      services.AddTransient<IEmailSender, EmailSender>();

      services.AddHsts(options =>
      {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(30);
      });

      services.AddHttpsRedirection(options =>
      {
        options.HttpsPort = 443;
      });

      services.AddResponseCompression(options =>
      {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();        
      });

      services.Configure<BrotliCompressionProviderOptions>(options =>
      {
        options.Level = CompressionLevel.Fastest;
      });

      services.Configure<GzipCompressionProviderOptions>(options =>
      {
        options.Level = CompressionLevel.Fastest;
      });

      services.AddMemoryCache();

      services.AddApplicationInsightsTelemetry(Configuration);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseHealthChecks("/health");

      app.UseResponseCompression();
      //app.UseHttpsRedirection();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
      }

      //app.UseStatusCodePages(); // App Insughts shows 404 URL
      app.UseStatusCodePagesWithReExecute("/Home/Error"); // App Insights shows /Home/Error

      if (!env.IsProduction())
      {
        var options = new RewriteOptions()
          .AddRewrite(@"^robots.txt", "robots.staging.txt", true)
          .AddRewrite(@"^data/", "/Home/Error", true);

        app.UseRewriter(options);
      }

      var provider = new FileExtensionContentTypeProvider();
      provider.Mappings[".webmanifest"] = "application/x-web-app-manifest+json";

      var cachePeriod = !env.IsProduction() ? "600" : "2592000";

      app.Use(async (context, next) =>
      {
        context.Response.Headers.Clear();
        context.Response.Headers.Add("Content-Security-Policy", "connect-src 'self' https://dc.services.visualstudio.com; default-src 'self' https://*.google.com; font-src https://fonts.googleapis.com/ https://fonts.gstatic.com/; img-src 'self' data: https://*.google.com https://*.googleapis.com https://*.googleusercontent.com https://*.gstatic.com https://www.google-analytics.com https://stats.g.doubleclick.net; script-src 'self' 'unsafe-inline' 'unsafe-eval' https://www.googletagmanager.com https://www.google-analytics.com https://*.google.com https://*.msecnd.net; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://www.google.com");
        context.Response.Headers.Add("Feature-Policy", "geolocation *");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

        await next();
      });

      app.UseStaticFiles(new StaticFileOptions
      {
        ContentTypeProvider = provider,
        OnPrepareResponse = ctx =>
        {
          ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
        }
      });

      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        if (env.IsProduction())
        {
          routes.MapRoute(
            name: "Account",
            template: "Account/{*url}",
            defaults: new { controller = "Home", action = "Error" });

          routes.MapRoute(
            name: "Manage",
            template: "Manage/{*url}",
            defaults: new { controller = "Home", action = "Error" });
        }

        routes.MapRoute(
          name: "Join",
          template: "Join",
          defaults: new { controller = "Home", action = "Join" });

        routes.MapRoute(
          name: "About",
          template: "About",
          defaults: new { controller = "Home", action = "About" });

        routes.MapRoute(
          name: "Contact",
          template: "Contact",
          defaults: new { controller = "Home", action = "Contact" });

        routes.MapRoute(
          name: "Search",
          template: "Search/{query?}/{pageNumber?}",
          defaults: new { controller = "Search", action = "Index" });

        routes.MapRoute(
          name: "Calendars",
          template: "Components/Calendar/Calendars",
          defaults: new { controller = "Calendar", action = "GetCalendars" });

        routes.MapRoute(
          name: "Events",
          template: "Components/Calendar/Events",
          defaults: new { controller = "Calendar", action = "GetEvents" });

        routes.MapRoute(
          name: "Documents",
          template: "Documents/{documentId?}",
          defaults: new { controller = "Resources", action = "Documents" });

        routes.MapRoute(
          name: "DocumentList",
          template: "Components/Resources/Default/{documentId?}",
          defaults: new { controller = "Resources", action = "GetDocuments" });

        routes.MapRoute(
          name: "Leaders",
          template: "Leaders",
          defaults: new { controller = "Resources", action = "Leaders" });

        routes.MapRoute(
          name: "Uniforms",
          template: "Uniforms",
          defaults: new { controller = "Resources", action = "Uniforms" });

        routes.MapRoute(
          name: "Skits",
          template: "Skits/{skitName?}",
          defaults: new { controller = "Campfire", action = "Skits" });

        routes.MapRoute(
          name: "Songs",
          template: "Songs/{songName?}",
          defaults: new { controller = "Campfire", action = "Songs" });

        routes.MapRoute(
          name: "Ceremonies",
          template: "Ceremonies/{ceremonyName?}",
          defaults: new { controller = "Campfire", action = "Ceremonies" });

        routes.MapRoute(
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}