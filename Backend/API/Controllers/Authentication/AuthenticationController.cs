using API.Models;
using API.Services.Interface;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Models;
using Service.Models.Authentication.Login;
using Service.Models.Authentication.Register;
using Service.Models.Authentication.User;
using Service.Services;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;



namespace API.Controllers.Authentication
{

    [Route("AuthenticationController")]
    [ApiController]
    
    public class AuthenticationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
       private readonly SignInManager<ApplicationUser> _signInManager;
       
        private readonly Service.Services.IEmailService _emailService;
        private readonly IUserManagement _userManagement;
        private readonly ICartService _cartService;
        public AuthenticationController(UserManager<ApplicationUser>userManager, RoleManager<IdentityRole> roleManager, Service.Services.IEmailService emailService, IConfiguration configuration,SignInManager<ApplicationUser> signInManager,IUserManagement userManagement, ICartService cartService)
        {
            _userManager = userManager; 
          
            _emailService = emailService;
          
            _userManagement = userManagement;
            _cartService = cartService;

        }

        [HttpPost]
        public async Task<IActionResult> Register(SignUp registerUser)
        {
            int cartId = await _cartService.AddToCartAsync();

            var user = new Users
            {
                UserName = registerUser.UserName,
                UserEmail = registerUser.Email,
                UserPassword = registerUser.Password,
                CartId = cartId
            };

            await _cartService.AddUser(user);

            var tokenResponse = await _userManagement.CreateUserWithTokenAsync(registerUser);
             if(tokenResponse.IsSuccess)
            {
                
                

                await _userManagement.AssignRoleToUserAsync(registerUser.Roles,tokenResponse.Response.User);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new {tokenResponse.Response.Token, email = registerUser.Email }, Request.Scheme);
                var message = new Message(new string[] { registerUser.Email! }, "Confirmation email link", confirmationLink);
                _emailService.SendEmail(message);
                // Assuming you have a method to retrieve claims from the ClaimsPrincipal
                
                
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email verfication mail sent" });
            }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response {Message = tokenResponse.Message,IsSuccess = false});

        }

        

        [HttpGet("Test Send email")]
        public IActionResult TestEmail()
        {
            var message = new Message(new string[] { "guptakhushboo81537@gmail.com" }, "test", "<h1>Testing</h1>");
            _emailService.SendEmail(message);
            return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email sent Successfully" });
        }

        [HttpGet("ConfirmedEmail")]
        public async Task<IActionResult> ConfirmEmail(string token ,string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user!= null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if(result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email Verified Successfully" });                   
                }
            }
            return  StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User does not exist" });

        }

        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login(Login login)
        {
           
           
            //checking user
            var loginOtpResponse = await _userManagement.GetOtpByLoginAsync(login);
            if(loginOtpResponse.Response! != null)
            {
                var user = loginOtpResponse.Response.User;
                if (user.TwoFactorEnabled)
                {
                    var token = loginOtpResponse.Response.Token;

                    var message = new Message(new string[] { user.Email! }, "OTP Confrimation", token);
                    _emailService.SendEmail(message);

                    return StatusCode(StatusCodes.Status200OK,
                     new Response {IsSuccess = loginOtpResponse.IsSuccess, Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}" });
                }
                if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
                {
                        var serviceResponse = await _userManagement.GetJwtTokenAsync(user);

                        return Ok(serviceResponse);                  

                }
            }
           
            return Unauthorized();
        }


        [HttpPost]
        [Route("Login-2FA")]
        public async Task<IActionResult> LoginWithOTP(string code, string email)
        {
        
            var jwt = await _userManagement.LoginUserWithJWTokenAsync(code, email);
            // var signIn = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);
            if (jwt.IsSuccess)
            {
                return Ok(jwt);

            }

            return StatusCode(StatusCodes.Status404NotFound, new Response { Status = $"Invalid OTP" });
        }

        [HttpPost]
        [Route("Refresh-Token")]
        public async Task<IActionResult> RefreshToken(LoginResponse accessToken)
        {
            var jwt = await _userManagement.RenewAccessTokenAsync(accessToken);
            if(jwt.IsSuccess)
            {
                return Ok(jwt);
            }
            return StatusCode(StatusCodes.Status404NotFound,
                new Response { Status = "Success", Message = $"Invalid code" });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgotPasswordlink = Url.Action(nameof(ResetPassword), "Authentication", new { token, email = user.Email }, Request.Scheme);
                var message = new Message(new string[] { user.Email! }, "Forgot Password Link", forgotPasswordlink! );
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK, new Response { Status = $"Password Changed request is sent on mail {user.Email}" });
            }
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = $"We have sent an OTP to your email {user.Email}" });
           }

        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var model = new ResetPassword {Token = token, Email = email};
            return Ok(new {model});
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("rest-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if(user!= null)
            {

                var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if(!resetPassResult.Succeeded)
                {
                    foreach(var error in resetPassResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return Ok(ModelState);
                }
          
                return StatusCode(StatusCodes.Status200OK, new Response { Status = $"Reset Password Link has been sent on {user.Email}" });
            }
            return StatusCode(StatusCodes.Status400BadRequest, StatusCode(StatusCodes.Status200OK, new Response { Status = "Couldn't find link to email" }));
        }

                      
                                                                           
      
       

    }
}
