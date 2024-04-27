using System.ComponentModel.DataAnnotations;

namespace Service.Models.Authentication.Register
{
    public class SignUp
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email Address is required")]
        public string Email{ get; set;}

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set;}

      
        public List<string>? Roles {  get; set;}    

        /*[Required(ErrorMessage = "ConfirmPassword is required")]
        public string ConfirmPassword { get; set;}  */


    }
}
