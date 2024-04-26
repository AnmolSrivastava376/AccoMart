using API.Models.Authentication;
using API.Models.Authentication.Register;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;
using Service.Models;
using Service.Services;

namespace API.Controllers.Authentication
{
    [Route("AuthenticationController")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;    
        private readonly RoleManager<IdentityRole> _roleManager;    
        private readonly IConfiguration _configuration;
        private readonly Service.Services.IEmailService _emailService;   
        public AuthenticationController(UserManager<IdentityUser>userManager, RoleManager<IdentityRole> roleManager, Service.Services.IEmailService emailService)
        {
            _userManager = userManager; 
            _roleManager = roleManager;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(SignUp register, string role)
        {
                
                var userExist = await _userManager.FindByEmailAsync(register.Email);
                if (userExist != null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = "Error", Message = "User Already Exists" });
                }

                var user = new IdentityUser
                {
                    Email = register.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = register.UserName,
                };

            if(await _roleManager.RoleExistsAsync(role)) {
                var result = await _userManager.CreateAsync(user, register.Password);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = "Error", Message = "Failed to Create" });

                }

                //Add Role to User
                await _userManager.AddToRoleAsync(user, role);

                //Add Token

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = user.Email }, Request.Scheme);
                var message = new Message(new string[] { user.Email!}, "Confirmation email link", confirmationLink);
                _emailService.SendEmail(message);

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "User created and email sent Successfully" });

            }
            else
            {
                return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = "Error", Message = "This Role does not exist" });
            }  
        }

        [HttpGet]
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

    }
}
