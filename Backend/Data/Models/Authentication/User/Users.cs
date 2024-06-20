using System.ComponentModel.DataAnnotations;

namespace Data.Models.Authentication.User
{
    public class Users
    {
        public string UserId { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "UserPassword is required")]
        public string UserPassword { get; set; }

        [Required(ErrorMessage = "UserEmail is required")]
        public string UserEmail { get; set; }
        public int CartId { get; set; }

    }
}
