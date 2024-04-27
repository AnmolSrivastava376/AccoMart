
using Microsoft.AspNetCore.Identity;
using Service.Models;
using Service.Models.Authentication.Register;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Service.Models.Authentication.User;

namespace Service.Services
{
    public class UserManagement : IUserManagement
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        
        public UserManagement(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;       
            _configuration = configuration;
            _signInManager = signInManager;
        }
        public async Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(SignUp register)
        {
            var userExist = await _userManager.FindByEmailAsync(register.Email);
            if (userExist != null)
            {
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 403, Message = "User already exists" };
            }

            IdentityUser user = new ()
            {
                Email = register.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = register.UserName,
                TwoFactorEnabled = true,
            };

            
                var result = await _userManager.CreateAsync(user, register.Password);
                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);   
                    return new ApiResponse<CreateUserResponse> {Response = new CreateUserResponse() { User = user, Token = token  }, IsSuccess = true, StatusCode = 201, Message = "User Created "};

                }


            else

                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 500, Message = "User Failed to create"};

        }

        public async Task<ApiResponse<List<string>>> AssignRoleToUserAsync(List<string> roles, IdentityUser user)
        {
            var assignedRole = new List<string>();
            foreach(var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    if (!await _userManager.IsInRoleAsync(user, role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                        assignedRole.Add(role); 
                    }
                }

            }
            return new ApiResponse<List<string>> {  IsSuccess=true,StatusCode= 200, Message = "Role Assigned",Response=assignedRole};

        }
    }
}
