using System.ComponentModel.DataAnnotations;

namespace Service.Models.Authentication.Login
{
    public class Login
    {

        [EmailAddress]
        [Required(ErrorMessage = "Email Address is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
