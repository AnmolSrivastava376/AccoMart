using System.ComponentModel.DataAnnotations;

namespace Data.Models.ResetPassword
{
    public class ResetPassword
    {
        [Required]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = null;

        [Required(ErrorMessage ="Email is required")]

        [EmailAddress(ErrorMessage ="Not a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Token not received")]
        public string Token { get; set; }
    }
}
