using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Models.Authentication.Register;
using Microsoft.AspNetCore.Identity;
using Service.Models.Authentication.User;
namespace Service.Services
{
    public interface IUserManagement
    {
        Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(SignUp register);
        Task<ApiResponse<List<string>>> AssignRoleToUserAsync(List<string> roles, IdentityUser user); 
    }
}
