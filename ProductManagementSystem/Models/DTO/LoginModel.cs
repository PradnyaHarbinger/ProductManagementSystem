using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.Models.DTO
{
    public class LoginModel
    {
        public LoginModel()
        {
            Email = "";
            Password = "";
        }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

    }
}
