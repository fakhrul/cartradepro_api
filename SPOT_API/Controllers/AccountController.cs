﻿using Microsoft.AspNetCore.Authorization;
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


                var role = await _context.Roles
                    .Include(c=> c.RoleModulePermissions)
                    .ThenInclude(c=> c.Module)
                    .Include(c=> c.RoleSubModulePermissions)
                    .ThenInclude(c=> c.SubModule)
                    .ThenInclude(c=> c.Module)
                    .Where(c => c.Name == user.Role)
                    .FirstOrDefaultAsync();

                foreach(var subModule in role.RoleSubModulePermissions)
                {
                    subModule.SubModule.Module.SubModules = null;
                    subModule.SubModule.RoleModulePermissions = null;
                    subModule.Role = null;
                }
                foreach(var module in role.RoleModulePermissions)
                {
                    module.Role = null;
                    module.Module.SubModules = null;
                }
                var roleModulePermissionList = role.RoleModulePermissions.ToList();
                foreach(var r in roleModulePermissionList)
                {
                    r.Role = null;
                }
                var roleSubModulePermissionsList = role.RoleSubModulePermissions.ToList();
                foreach (var r in roleSubModulePermissionsList)
                {
                    r.Role = null;
                }

                var userDto = new UserDto
                {
                    DisplayName = user.DisplayName,
                    Image = null,
                    Token = _tokenService.CreateToken(user),
                    UserName = user.UserName,
                    Role = user.Role,
                    //Roles = roles,
                    RoleModulePermissions = roleModulePermissionList,
                    RoleSubModulePermissions = roleSubModulePermissionsList
                };

                return userDto;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            return Ok("Registration success");
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

            Profile profile = new Profile
            {
                FullName = "Your Full Name",
                Email = registerDto.Email,
                Role = "TenantAdmin",
            };

            await _context.Profiles.AddAsync(profile);

            var user = new AppUser
            {
                DisplayName = profile.FullName,
                Email = profile.Email,
                UserName = profile.Email,
                ProfileId = profile.Id,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest("Problem registering user");


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
