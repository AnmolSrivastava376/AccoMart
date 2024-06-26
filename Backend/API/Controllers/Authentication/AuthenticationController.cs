﻿using Data.Models;
using Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Data.Models.Authentication.Login;
using Data.Models.Authentication.Register;
using Data.Models.Authentication.User;
using Data.Models.ResetPassword;
using Service.Services;


namespace API.Controllers.Authentication
{

    [Route("AuthenticationController")]
    [ApiController]

    public class AuthenticationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IEmailService _emailService;
        private readonly IUserManagement _userManagement;
        private readonly ICartService _cartService;
        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IEmailService emailService, IConfiguration configuration, SignInManager<ApplicationUser> signInManager, IUserManagement userManagement, ICartService cartService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _userManagement = userManagement;
            _cartService = cartService;

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(SignUp registerUser)
        {

            var tokenResponse = await _userManagement.CreateUserWithTokenAsync(registerUser);
            if (tokenResponse.IsSuccess)
            {
                await _userManagement.AssignRoleToUserAsync(registerUser.Roles, tokenResponse.Response.User);
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "You have been registered" });
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Message = tokenResponse.Message, IsSuccess = false });

        }


        [HttpGet("ConfirmedEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email Verified Successfully" });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User does not exist" });

        }


        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login(Login login)
        {
            string password = login.Password;
            string email = login.Email;
            var jwt = await _userManagement.LoginUserWithJWTokenAsync(password, email);
            if (jwt.IsSuccess)
            {
                return Ok(jwt);

            }

            return StatusCode(StatusCodes.Status404NotFound, new Response { Status = $"Failure", Message = jwt.Message });
        }


        [HttpPost]
        [Route("LoginByOtp")]

        public async Task<IActionResult> LoginForgotPassword(string email)
        {
            var loginOtpResponse = await _userManagement.GetOtpByLoginAsync(email);
            if (loginOtpResponse.Response! != null)
            {
                var user = loginOtpResponse.Response.User;

                var token = loginOtpResponse.Response.Token;

                var message = new Message(new string[] { user.Email! }, "OTP Confrimation", token);
                _emailService.SendEmail(message);

                return StatusCode(StatusCodes.Status200OK,
                 new Response { IsSuccess = loginOtpResponse.IsSuccess, Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}" });


            }
            return Unauthorized();
        }


        [HttpPost]
        [Route("Login-2FA")]
        public async Task<IActionResult> LoginWithOTP(string code, string email)
        {
            var jwt = await _userManagement.LoginUserWithJWTokenAsyncForgotPassword(code, email);
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
            if (jwt.IsSuccess)
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
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var forgotPasswordlink = $"https://agreeable-coast-00adf000f.5.azurestaticapps.net/home/reset-password?token={encodedToken}&email={Uri.EscapeDataString(user.Email)}";
                var message = new Message(new string[] { user.Email! }, "Forgot Password Link", forgotPasswordlink);
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK, new Response { Status = $"Password change request has been sent to {user.Email}" });
            }
            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = $"We have sent an OTP to your email {email}" });
        }


        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var model = new ResetPassword { Token = token, Email = email };
            return Ok(new { model });
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (!resetPassResult.Succeeded)
                {
                    foreach (var error in resetPassResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                return StatusCode(StatusCodes.Status200OK, new Response { Status = $"Password has been reset" });
            }
            return StatusCode(StatusCodes.Status400BadRequest, StatusCode(StatusCodes.Status200OK, new Response { Status = "Couldn't find link to email" }));
        }


    }
}