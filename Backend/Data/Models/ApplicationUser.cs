using Microsoft.AspNetCore.Identity;

namespace Data.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public int CartId { get; set; }

    }
}
