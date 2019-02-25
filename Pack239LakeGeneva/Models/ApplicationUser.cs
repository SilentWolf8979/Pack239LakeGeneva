using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Pack239LakeGeneva.Models
{
  // Add profile data for application users by adding properties to the ApplicationUser class
  public class ApplicationUser : IdentityUser
  {
    //
    // Summary:
    //     Gets or sets a first name for the user.
    [ProtectedPersonalData]
    public virtual string FirstName { get; set; }

    //
    // Summary:
    //     Gets or sets a last name for the user.
    [ProtectedPersonalData]
    public virtual string LastName { get; set; }
  }
}
