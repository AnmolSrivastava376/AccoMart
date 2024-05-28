using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class ResetPassword
    {
        [Required]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = null;

        public string Email { get; set; }   
        public string Token { get; set; }
    }
}
