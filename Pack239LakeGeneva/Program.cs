using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Pack239LakeGeneva
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, config) =>
        {
          config.AddJsonFile("client_secrets.json", optional: false, reloadOnChange: true);
        })
            .UseStartup<Startup>();
  }
}