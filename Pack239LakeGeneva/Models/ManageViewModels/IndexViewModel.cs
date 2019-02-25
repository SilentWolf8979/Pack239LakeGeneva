using System.ComponentModel.DataAnnotations;

namespace Pack239LakeGeneva.Models.ManageViewModels
{
  public class IndexViewModel
  {
    public string Username { get; set; }

    public bool IsEmailConfirmed { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }

    public string StatusMessage { get; set; }
  }
}
