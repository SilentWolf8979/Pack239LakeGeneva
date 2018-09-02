using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pack239LakeGeneva.Data;
using Pack239LakeGeneva.Models;
using Pack239LakeGeneva.Services;
using Microsoft.AspNetCore.Mvc;

namespace Pack239LakeGeneva
{
  public class Startup
  {
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

      services.AddIdentity<ApplicationUser, IdentityRole>()
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();

      // Add application services.
      services.AddTransient<IEmailSender, EmailSender>();

      services.AddHttpsRedirection(options =>
      {
        options.HttpsPort = 443;
      });

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseBrowserLink();
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStatusCodePages();
      app.UseStatusCodePagesWithReExecute("/Home/Error");

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "About",
          template: "About",
          defaults: new { controller = "Home", action = "About" });

        routes.MapRoute(
          name: "Contact",
          template: "Contact",
          defaults: new { controller = "Home", action = "Contact" });

        routes.MapRoute(
          name: "Events",
          template: "Components/Calendar/Default",
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

      //await Initializer.initialize(roleManager);
    }
  }
}