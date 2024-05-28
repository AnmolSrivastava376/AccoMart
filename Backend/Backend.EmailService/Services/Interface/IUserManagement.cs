using Service.Models;
using Data.Models.Authentication.User;
using Data.Models.Authentication.Register;
namespace Service.Services.Interface
{
    public interface IUserManagement
    {
        Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(SignUp register);
        Task<ApiResponse<List<string>>> AssignRoleToUserAsync(List<string> roles, ApplicationUser user);
        Task<ApiResponse<LoginOtpResponse>> GetOtpByLoginAsync(string email);
        Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(ApplicationUser user);
        Task<ApiResponse<LoginResponse>> LoginUserWithJWTokenAsync(string password, string email);
        Task<ApiResponse<LoginResponse>> LoginUserWithJWTokenAsyncForgotPassword(string code, string email);
        Task<ApiResponse<LoginResponse>> RenewAccessTokenAsync(LoginResponse token);
    }
}
