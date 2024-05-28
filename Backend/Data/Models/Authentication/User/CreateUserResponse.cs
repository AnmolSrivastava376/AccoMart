namespace Data.Models.Authentication.User
{
    public class CreateUserResponse
    {
        public string Token { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

    }
}
