
using Microsoft.AspNetCore.Identity;
using Service.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using Service.Services.Interface;
using Microsoft.Data.SqlClient;
using Data.Models.Address;
using Data.Models.Authentication.Register;
using Data.Models.Authentication.User;


namespace Service.Services
{
    public class UserManagement : IUserManagement
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ICartService _cartService;
        private readonly string connectionstring = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
        private readonly string jwt_secret = Environment.GetEnvironmentVariable("JWT_SECRET");


        public UserManagement(UserManager<ApplicationUser> userManager, IConfiguration configuration, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ICartService cartService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _cartService = cartService;
        }


        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwt_secret));
            _ = int.TryParse(_configuration["JWT:TokenValidiyInMinutes"], out int tokenValidityInMinutes);
            tokenValidityInMinutes += 10;  // increasing the token validity time
            var expirationTimeUtc = DateTime.UtcNow.AddMinutes(tokenValidityInMinutes);
            var localTimeZone = TimeZoneInfo.Local;
            var expirationTimeInLocalTimeZone = TimeZoneInfo.ConvertTimeFromUtc(expirationTimeUtc, localTimeZone);
            var token = new JwtSecurityToken(

                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: expirationTimeInLocalTimeZone,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)

                );
            return token;

        }


        private string GenerateRefreshToken()
        {
            var randomNumber = new Byte[64];
            var range = RandomNumberGenerator.Create();
            range.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetClaimsPrincipal(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters

            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
            return principal;

        }


        public async Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(SignUp register)
        {
            var userExist = await _userManager.FindByEmailAsync(register.Email);
            var userNameExist = await _userManager.FindByNameAsync(register.UserName);
            if (userExist != null)
            {
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 403, Message = "User has already been registered through this email" };
            }
            if (userNameExist != null)
            {
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 403, Message = "This username has already been taken" };
            }

            int cartId = await _cartService.AddToCartAsync();


            ApplicationUser user = new()
            {
                Email = register.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = register.UserName,
                CartId = cartId,
                TwoFactorEnabled = false,
                EmailConfirmed = true
            };


            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                return new ApiResponse<CreateUserResponse> { Response = new CreateUserResponse() { User = user, Token = token }, IsSuccess = true, StatusCode = 201, Message = "User Created " };

            }


            else
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 500, Message = "Registration failed" };

        }

        public async Task<ApiResponse<List<string>>> AssignRoleToUserAsync(List<string> roles, ApplicationUser user)
        {
            var assignedRole = new List<string>();
            foreach (var role in roles)
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
            return new ApiResponse<List<string>> { IsSuccess = true, StatusCode = 200, Message = "Role Assigned", Response = assignedRole };

        }

        public async Task<ApiResponse<LoginOtpResponse>> GetOtpByLoginAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                /*await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, login.Password, false, true);*/

                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                return new ApiResponse<LoginOtpResponse>
                {
                    Response = new LoginOtpResponse()
                    {
                        User = user,
                        Token = token,
                        IsTwoFactorEnable = user.TwoFactorEnabled
                    },
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "OTP sent to email "
                };
            }
            else
            {
                return new ApiResponse<LoginOtpResponse>
                {

                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "User does not exist"
                };
            }

        }

        public async Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(ApplicationUser user)
        {
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim("UserName", user.UserName),
        new Claim("UserId", user.Id),
        new Claim("CartId", user.CartId.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

            var addressId = GetAddressIdByUserId(user.Id);
            if (addressId != null)
            {
                authClaims.Add(new Claim("AddressId", addressId));
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim("Role", role));
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtToken = GetToken(authClaims); //access token
            var refreshToken = GenerateRefreshToken();
            _ = int.TryParse(_configuration["JWT:RefreshTokenValidity"], out int refreshTokenValidity);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenValidity);
            await _userManager.UpdateAsync(user);

            return new ApiResponse<LoginResponse>
            {
                Response = new LoginResponse()
                {
                    AccessToken = new TokenType()
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        ExpiryTokenDate = jwtToken.ValidTo
                    },
                    RefreshToken = new TokenType()
                    {
                        Token = user.RefreshToken,
                        ExpiryTokenDate = (DateTime)user.RefreshTokenExpiry
                    }
                },
                IsSuccess = true,
                StatusCode = 200,
                Message = $"Login Successful"
            };
        }
        private string GetAddressIdByUserId(string userId)
        {
            int addressId = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT * FROM Addresses WHERE UserId = @UserId", connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                AddressModel address = new AddressModel
                                {
                                    Street = reader.GetString(reader.GetOrdinal("Street")),
                                    City = reader.GetString(reader.GetOrdinal("City")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    State = reader.GetString(reader.GetOrdinal("States")),
                                    ZipCode = reader.GetString(reader.GetOrdinal("ZipCode"))

                                };
                                addressId = (int)command.ExecuteScalar();

                                return addressId.ToString();
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ApiResponse<LoginResponse>> LoginUserWithJWTokenAsync(string password, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ApiResponse<LoginResponse>()
                {
                    Response = new LoginResponse(),
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Email not registered"
                };
            }

            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, true, false);
                if (signInResult.Succeeded)
                {
                    return await GetJwtTokenAsync(user);
                }
            }

            return new ApiResponse<LoginResponse>()
            {
                Response = new LoginResponse(),
                IsSuccess = false,
                StatusCode = 400,
                Message = "Otp is incorrect"
            };
        }

        public async Task<ApiResponse<LoginResponse>> LoginUserWithJWTokenAsyncForgotPassword(string code, string email)
        {

            var user = await _userManager.FindByEmailAsync(email);
            var signIn = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", code);
            //var signIn = await _signInManager.TwoFactorSignInAsync("Email", code, true, false);
            if (user == null)
            {
                return new ApiResponse<LoginResponse>()
                {
                    Response = new LoginResponse(),
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Email not registered"
                };
            }
            if (signIn)
            {
                if (user != null)
                {
                    return await GetJwtTokenAsync(user);
                }
            }
            return new ApiResponse<LoginResponse>()
            {

                Response = new LoginResponse()
                {

                },
                IsSuccess = false,
                StatusCode = 400,
                Message = $"Invalid Otp"
            };
        }

        public async Task<ApiResponse<LoginResponse>> RenewAccessTokenAsync(LoginResponse tokens)
        {
            var accessToken = tokens.AccessToken;
            var refreshToken = tokens.RefreshToken;
            var principal = GetClaimsPrincipal(accessToken.Token);
            var user = await _userManager.FindByNameAsync(principal.Identity.Name);
            if (refreshToken.Token != user.RefreshToken && refreshToken.ExpiryTokenDate <= DateTime.Now)
            {
                return new ApiResponse<LoginResponse>
                {

                    IsSuccess = false,
                    StatusCode = 400,
                    Message = $"Token invalid or expired"
                };
            }
            var response = await GetJwtTokenAsync(user);
            return response;


        }


    }
}