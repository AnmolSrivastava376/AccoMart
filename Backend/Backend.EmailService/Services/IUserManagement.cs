using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Models.Authentication.Register;
using Microsoft.AspNetCore.Identity;
using Service.Models.Authentication.User;
using Service.Models.Authentication.Login;
using Data.Models.Authentication.User;
namespace Service.Services
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
