using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SPOT_API.DTOs;
using SPOT_API.Models;
using SPOT_API.Persistence;
using SPOT_API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Controllers
{
    //[AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(SpotDBContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TokenService tokenService)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }


        

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                    return Unauthorized();

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (!result.Succeeded)
                    return Unauthorized();


                return new UserDto
                {
                    DisplayName = user.DisplayName,
                    Image = null,
                    Token = _tokenService.CreateToken(user),
                    UserName = user.UserName,
                    Role = user.Role,

                };


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
                ModelState.AddModelError("email", "Email taken");
                return ValidationProblem();
            }
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.UserName))
            {
                ModelState.AddModelError("username", "Username taken");
                return ValidationProblem();
            }

            if (string.IsNullOrEmpty(registerDto.IdNo))
            {
                ModelState.AddModelError("idNo", "Id no cannot be empty");
                return ValidationProblem();
            }
            if (string.IsNullOrEmpty(registerDto.Role))
            {
                ModelState.AddModelError("role", "Undefined Role");
                return ValidationProblem();
            }
            if (string.IsNullOrEmpty(registerDto.TenantCode))
            {
                ModelState.AddModelError("tenantCode", "Unknown tenant");
                return ValidationProblem();
            }


            Profile profile = new Profile
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                Role = registerDto.Role,
              
                //StaffNo = registerDto.IdNo,
            };

            //if (registerDto.Role.ToLower() == "staff")
            //{
            //    profile.StaffNo = registerDto.IdNo;
            //}
            //else if (registerDto.Role.ToLower() == "visitor")
            //{
            //    profile.MyKad = registerDto.IdNo;
            //}

            await _context.Profiles.AddAsync(profile);

            var user = new AppUser
            {
                DisplayName = registerDto.FullName,
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                ProfileId = profile.Id,

            };

            //AppUser user = new AppUser
            //{
            //    DisplayName = "Tenant",
            //    UserName = "tenant" + count.ToString(),
            //    ProfileId = userAdmin.Id,
            //    Email = "tenant" + count.ToString() + "@email.com",
            //    TenantCode = tenant.Code,
            //    TenantId = tenant.Id,
            //    //Role = 
            //};


            var result = await _userManager.CreateAsync(user, "Qwerty@123");

            if (!result.Succeeded) return BadRequest("Problem registering user");

            //var origin = Request.Headers["origin"];
            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            //var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
            //var message = $"<p>Please click the below link to verify your email address:</p><p><a href='{verifyUrl}'>Click to verify email</a></p>";

            //await _emailSender.SendEmailAsync(user.Email, "Please verify email", message);

            return Ok("Registration success");
        }

        [AllowAnonymous]
        [HttpPost("verifyEmail")]
        public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized();
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded) return BadRequest("Could not verify email address");

            return Ok("Email confirmed - you can now login");
        }


        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            await SetRefreshToken(user);
            return CreateUserObject(user);
        }

        //[AllowAnonymous]
        //[HttpGet("resendEmailConfirmationLink")]
        //public async Task<IActionResult> ResendEmailConfirmationLink(string email)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);

        //    if (user == null) return Unauthorized();

        //    var origin = Request.Headers["origin"];
        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        //    var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
        //    var message = $"<p>Please click the below link to verify your email address:</p><p><a href='{verifyUrl}'>Click to verify email</a></p>";

        //    await _emailSender.SendEmailAsync(user.Email, "Please verify email", message);

        //    return Ok("Email verification link resent");
        //}


        [Authorize]
        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var user = await _userManager.Users
                .Include(r => r.RefreshTokens)
                
                .FirstOrDefaultAsync(x => x.UserName == User.FindFirstValue(ClaimTypes.Name));

            if (user == null) return Unauthorized();

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive) return Unauthorized();

            return CreateUserObject(user);
        }

        private async Task SetRefreshToken(AppUser user)
        {
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }

        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                
                Token = _tokenService.CreateToken(user),
                UserName = user.UserName
            };
        }
    }
}
