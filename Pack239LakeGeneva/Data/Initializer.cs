using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Pack239LakeGeneva.Data
{
  public static class Initializer
  {
    public static async Task initialize(RoleManager<IdentityRole> roleManager)
    {
      if (!await roleManager.RoleExistsAsync("Admin"))
      {
        var users = new IdentityRole("Admin");
        await roleManager.CreateAsync(users);
      }

      if (!await roleManager.RoleExistsAsync("User"))
      {
        var users = new IdentityRole("User");
        await roleManager.CreateAsync(users);
      }

      if (!await roleManager.RoleExistsAsync("Manager"))
      {
        var users = new IdentityRole("Manager");
        await roleManager.CreateAsync(users);
      }
    }
  }
}
