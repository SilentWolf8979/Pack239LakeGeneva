using System;
using System.IO.Compression;
using System.Linq;

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

using Pack239LakeGeneva.Data;
using Pack239LakeGeneva.Models;
using Pack239LakeGeneva.Services;

namespace Pack239LakeGeneva
{
  public class Startup
  {
    private IConfiguration Configuration { get; }
    private IHostingEnvironment _env { get; }

    public Startup(IConfiguration configuration, IHostingEnvironment environment)
    {
      Configuration = configuration;
      _env = environment;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      if (_env.IsDevelopment())//(_env.IsStaging() || _env.IsProduction())
      {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("AzureConnection")));
      }
      else
      {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
      }

      services.AddIdentity<ApplicationUser, IdentityRole>()
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();

      // Add application services.
      services.AddTransient<IEmailSender, EmailSender>();

      services.AddHsts(options =>
      {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(1);
      });

      services.AddHttpsRedirection(options =>
      {
        options.HttpsPort = 443;
      });

      services.AddResponseCompression(options =>
      {
        options.EnableForHttps = true;
        options.Providers.Add<GzipCompressionProvider>();
      });

      services.Configure<GzipCompressionProviderOptions>(options =>
      {
        options.Level = CompressionLevel.Fastest;
      });

      services.AddMemoryCache();

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      services.AddApplicationInsightsTelemetry(Configuration);

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseResponseCompression();
      //app.UseHttpsRedirection();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      //app.UseStatusCodePages(); // App Insughts shows 404 URL
      app.UseStatusCodePagesWithReExecute("/Home/Error"); // App Insights shows /Home/Error

      if (!env.IsProduction())
      {
        var options = new RewriteOptions()
          .AddRewrite(@"^robots.txt", "robots.staging.txt", true);

        app.UseRewriter(options);
      }

      var provider = new FileExtensionContentTypeProvider();
      provider.Mappings[".webmanifest"] = "application/x-web-app-manifest+json";

      var cachePeriod = !env.IsProduction() ? "600" : "604800";

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
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}