using Microsoft.AspNetCore.Identity;

namespace Data.Models.Authentication.User
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public int CartId { get; set; }

    }
}
