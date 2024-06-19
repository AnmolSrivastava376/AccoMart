using System.ComponentModel.DataAnnotations;

namespace Data.Models.Authentication.User
{
    public class Admin
    {
        [Required]
        public int AdminId { get; set; }
        [EmailAddress]
        public string AdminEmail { get; set; }
        [Required]
        public string AdminPassword { get; set; }
    }
}
