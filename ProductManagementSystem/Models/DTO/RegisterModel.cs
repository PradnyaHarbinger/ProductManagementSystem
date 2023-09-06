using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ProductManagementSystem.Models.DTO
{
    public class RegisterModel
    {
        public RegisterModel()
        {
            FirstName = "";
            LastName = "";
            Email = "";
            Password = "";
            ConfirmPassword = "";
        }
        
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }


        [EmailAddress]
        [Display(Name = "Email")]
        [Required]
        public string Email { get; set; }


        [StringLength(100, ErrorMessage = "The {0} must be {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmed password do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Required]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; } = "User";




    }
}
