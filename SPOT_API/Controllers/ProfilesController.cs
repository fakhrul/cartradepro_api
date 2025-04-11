using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.DTOs;
using SPOT_API.Models;
using SPOT_API.Persistence;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;

        public ProfilesController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
        }

        // GET: api/Profiles
        [HttpGet("ByOperator")]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfilesByOperator()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var objs = await _context.Profiles
                .Include(c => c.Leader)
                .Where(c => c.Role == "operator")
                .ToListAsync();

            foreach (var obj in objs)
            {
                obj.AppUser = null;
                if (obj.Leader != null)
                    obj.Leader.AppUser = null;
            }

            return objs;
        }


        // GET: api/Profiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfiles()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var objs = await _context.Profiles
                //.Include(c => c.Leader)
                //.Where(c => c.Role == "agent")
                .ToListAsync();
            foreach (var obj in objs)
            {
                obj.AppUser = null;
                //if (obj.Leader != null)
                //    obj.Leader.AppUser = null;
            }

            return objs;
        }

        // GET: api/Profiles
        [HttpGet("ByRoles/{role}")]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfiles(string role)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();


            return await _context.Profiles.Where(c => c.Role.ToLower() == role.ToLower())



                .OrderBy(c => c.FullName)
                .ToListAsync();

            //return await _context.Profiles.Where(c=>c.Role.ToLower() == role.ToLower()).ToListAsync();
        }



        // GET: api/Profiles
        [HttpGet("BySystemUserRoles")]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfileBySystemUserRoles()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            //if (user.IsSuperAdmin)
            //    return await _context.Profiles.ToListAsync();

            var profiles = await _context.Profiles
                .Where(c => c.AppUser.TenantId == user.TenantId)
                .Where(c => c.Role.ToLower() == "operation" || c.Role.ToLower() == "safety" || c.Role.ToLower() == "admin")
                //.Where(d => d.Department.TenantId == user.TenantId)
                //.Include(o => o.Department)
                //.Include(o => o.Device)
                .OrderBy(c => c.FullName)
                .ToListAsync();

            foreach (var profile in profiles)
            {
                if (profile.AppUser != null && profile.AppUser.Profile != null)
                    profile.AppUser.Profile = null;
            }
            return profiles;
            //return await _context.Profiles.Where(c=>c.Role.ToLower() == role.ToLower()).ToListAsync();
        }


        // GET: api/Profiles/5
        [HttpGet("BySystemAdmin/{id}")]
        public async Task<ActionResult<Profile>> GetProfileBySystemAdmin(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();


            var profile = await _context.Profiles
                .Include(c => c.AppUser)

                .FirstOrDefaultAsync(c => c.Id == id);

            if (profile == null)
            {
                return NotFound();
            }

            if (profile.AppUser != null && profile.AppUser.Profile != null)
                profile.AppUser.Profile = null;

            return profile;
        }


        [HttpGet("ByAdminTenantUser")]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfileByAdminTenantUser()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var profiles = await _context.Profiles
                .Where(c => c.Role == "admin")
                //.Where(c => c.TenantId != Guid.Empty)
                .Include(c => c.AppUser)
                .OrderBy(c => c.FullName)
                .ToListAsync();

            foreach (var profile in profiles)
            {
                if (profile.AppUser != null)
                    profile.AppUser.Profile = null;

            }
            return profiles;
        }

        // GET: api/Profiles/5
        [HttpGet("ByAdminTenantUser/{id}")]
        public async Task<ActionResult<Profile>> GetProfileByAdminTenantUser(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            //if (!user.IsSuperAdmin)
            //    return Unauthorized();

            var profile = await _context.Profiles
                .Where(c => c.Role == "admin")
                .Include(c => c.AppUser)
                //.Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (profile == null)
            {
                return NotFound();
            }

            if (profile.AppUser != null && profile.AppUser.Profile != null)
                profile.AppUser.Profile = null;

            return profile;
        }


        [HttpGet("ActiveProfileByTenant/{id}")]
        public async Task<ActionResult<IEnumerable<Profile>>> GetActiveProfileByTenant(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var profiles = await _context.Profiles


                //.Include(c=> c.Device)
                .ToListAsync();

            foreach (var profile in profiles)
            {
                if (profile.AppUser != null)
                    profile.AppUser.Profile = null;

            }

            //if (profile == null)
            //{
            //    return NotFound();
            //}

            //if (profile.AppUser != null && profile.AppUser.Profile != null)
            //    profile.AppUser.Profile = null;

            return profiles;
        }


        [HttpGet("ByCurrentLeaderProfile")]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfilesByCurrentLeaderProfile()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var profiles = await _context.Profiles
                .Where(c => c.LeaderId == user.ProfileId)
                .ToListAsync();

            foreach (var profile in profiles)
            {
                if (profile.AppUser != null)
                    profile.AppUser.Profile = null;
            }

            return profiles;
        }

        // GET: api/Profiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Profile>> GetProfile(Guid id)
        {
            var profile = await _context.Profiles
                .Include(c => c.AppUser)
                .Include(c => c.ProfilePackages)
                .Include("Leader.ProfilePackages")
                //.Include("ProfilePackages.Profile")
                .Include(c => c.Leader)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (profile == null)
            {
                return NotFound();
            }

            if (profile.AppUser != null && profile.AppUser.Profile != null)
                profile.AppUser.Profile = null;
            foreach (var packageProfile in profile.ProfilePackages)
            {
                if (packageProfile.Profile != null)
                {
                    packageProfile.Profile = null;

                }
            }

            if (profile.Leader != null)
                foreach (var packageProfile in profile.Leader.ProfilePackages)
                {
                    if (packageProfile.Profile != null)
                    {
                        packageProfile.Profile = null;

                    }
                }
            return profile;
        }



        // PUT: api/Profiles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfile(Guid id, Profile profile)
        {
            if (id != profile.Id)
            {
                return BadRequest();
            }

            _context.Entry(profile).State = EntityState.Modified;



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            catch (Exception ex)
            {
                throw ex;

            }





            return NoContent();
        }

        // POST: api/Profiles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Profile>> PostProfile(Profile profile)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();


                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();

                AppUser newAppUser = new AppUser
                {
                    DisplayName = profile.FullName,
                    UserName = profile.Email,
                    ProfileId = profile.Id,
                    Email = profile.Email,

                };


                await _userManager.CreateAsync(newAppUser, "Qwerty@123");

            }
            catch (Exception ex)
            {
            }

            if (profile.AppUser != null && profile.AppUser.Profile != null)
                profile.AppUser = null;

            return CreatedAtAction("GetProfile", new { id = profile.Id }, profile);
        }

        [HttpPost("ProfileAppUser")]
        public async Task<ActionResult<Profile>> PostProfileAppUser(ProfileDto profileDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            //if (!user.IsSuperAdmin)
            //    return Unauthorized();
            try
            {
                var profile = profileDto.Profile;
                if (string.IsNullOrEmpty(profile.Role))
                    profile.Role = "Sales";

                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();

                AppUser newAppUser = new AppUser
                {
                    DisplayName = profile.FullName,
                    UserName = profile.Email,
                    ProfileId = profile.Id,
                    Email = profile.Email,
                    //IsSuperAdmin = true,
                    Role = profile.Role,
                };

                await _userManager.CreateAsync(newAppUser, profileDto.PlainPassword);


                if (profile.AppUser != null && profile.AppUser.Profile != null)
                    profile.AppUser = null;
                profile.ProfilePackages = null;

                return CreatedAtAction("GetProfile", new { id = profile.Id }, profile);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPut("ProfileAppUser/{id}")]
        public async Task<IActionResult> PutProfileAppUser(Guid id, ProfileDto profileDto)
        {
            if (id != profileDto.Profile.Id)
            {
                return BadRequest();
            }

            _context.Entry(profileDto.Profile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                //var packageProfiles = await _context.ProfilePackages.Where(c => c.ProfileId == profileDto.Profile.Id).ToListAsync();
                //_context.ProfilePackages.RemoveRange(packageProfiles);
                //await _context.SaveChangesAsync();

                //foreach (var profilePackage in profileDto.Profile.ProfilePackages)
                //{
                //    profilePackage.ProfileId = profileDto.Profile.Id;
                //    _context.ProfilePackages.Add(profilePackage);
                //}


                //await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (profileDto.IsResetPassword && !string.IsNullOrEmpty(profileDto.PlainPassword))
            {
                var user = await _userManager.FindByEmailAsync(profileDto.Profile.Email);
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, profileDto.PlainPassword);
                var result = await _userManager.UpdateAsync(user);
            }

            var user2 = await _userManager.FindByEmailAsync(profileDto.Profile.Email);
            if(user2 != null)
            {
                if(user2.Role != profileDto.Profile.Role)
                {
                    user2.Role = profileDto.Profile.Role;
                    var result = await _userManager.UpdateAsync(user2);
                }
            }

            return NoContent();
        }


        [HttpGet("ByCurrentProfile")]
        public async Task<ActionResult<Profile>> GetProfileByCurrentProfile()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var profile = await _context.Profiles
                .Where(c => c.Email == user.Email)
                .OrderBy(c => c.FullName)
                .FirstOrDefaultAsync();

            if (profile == null)
            {
                return NotFound();
            }

            if (profile.AppUser != null && profile.AppUser.Profile != null)
                profile.AppUser.Profile = null;

            return profile;
        }


        [HttpGet("BySystemUsers")]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfileBySystemUsers()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            //if (user.IsSuperAdmin)
            //    return await _context.Profiles.ToListAsync();

            var profiles = await _context.Profiles
                .Where(c => c.AppUser.TenantId == user.TenantId)
                //.Where(c => c.Role.ToLower() == "operation" || c.Role.ToLower() == "safety" || c.Role.ToLower() == "admin")
                //.Where(d => d.Department.TenantId == user.TenantId)
                //.Include(o => o.Department)
                //.Include(o => o.Device)
                .OrderBy(c => c.FullName)
                .ToListAsync();

            foreach (var profile in profiles)
            {
                if (profile.AppUser != null && profile.AppUser.Profile != null)
                    profile.AppUser.Profile = null;
            }
            return profiles;
            //return await _context.Profiles.Where(c=>c.Role.ToLower() == role.ToLower()).ToListAsync();
        }

        [HttpGet("Sales")]
        public async Task<ActionResult<IEnumerable<Profile>>> GetSalesProfiles()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var salesProfiles = await _context.Profiles
                .Where(c => c.Role.ToLower() == "sales")
                .OrderBy(c => c.FullName)
                .ToListAsync();

            foreach (var profile in salesProfiles)
            {
                if (profile.AppUser != null && profile.AppUser.Profile != null)
                    profile.AppUser.Profile = null;
            }

            return salesProfiles;
        }


        //[HttpPost("BySystemUser")]
        //public async Task<ActionResult<Profile>> PostProfileBySystemUser(ProfileDto profileDto)
        //{
        //    var user = await _context.Users
        //        .Include(c => c.Profile)
        //        .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
        //    if (user == null)
        //        return Unauthorized();

        //    //if(user.IsSuperAdmin)
        //    //    return Unauthorized();

        //    if (user.Role != "admin")
        //    {
        //        if (user.Profile == null)
        //            return Unauthorized();
        //        if (user.Profile.Role != "admin")
        //            return Unauthorized();
        //    }
        //    //if (profile.Role.ToLower() != "super")
        //    //{
        //    //    if (profile.TenantId == null || profile.TenantId == Guid.Empty)
        //    //    {
        //    //        profile.TenantId = user.TenantId;
        //    //    }
        //    //}
        //    var profile = profileDto.Profile;


        //    if (string.IsNullOrEmpty(profileDto.PlainPassword))
        //        return BadRequest();

        //    _context.Profiles.Add(profile);
        //    await _context.SaveChangesAsync();

        //    //if (profile.Role.ToLower() == "super")
        //    //{
        //    AppUser newAppUser = new AppUser
        //    {
        //        DisplayName = profile.FullName,
        //        UserName = profile.Email,
        //        ProfileId = profile.Id,
        //        Email = profile.Email,

        //        IsSuperAdmin = false,
        //        Role = profile.Role,

        //    };
        //    //var userManager = services.GetRequiredService<UserManager<AppUser>>();

        //    if (profileDto.IsAzureAd)
        //        await _userManager.CreateAsync(newAppUser, "Qwerty@123");
        //    else
        //    {
        //        await _userManager.CreateAsync(newAppUser, profileDto.PlainPassword);
        //    }
        //    if (profile.AppUser != null && profile.AppUser.Profile != null)
        //        profile.AppUser = null;

        //    return CreatedAtAction("GetProfile", new { id = profile.Id }, profile);
        //}

        //[HttpPut("BySystemUser/{id}")]
        //public async Task<IActionResult> PutProfileBySystemUser(Guid id, ProfileDto profileDto)
        //{
        //    if (id != profileDto.Profile.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(profileDto.Profile).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ProfileExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    if (!profileDto.IsAzureAd && profileDto.IsResetPassword && !string.IsNullOrEmpty(profileDto.PlainPassword))
        //    {
        //        var user = await _userManager.FindByEmailAsync(profileDto.Profile.Email);

        //        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, profileDto.PlainPassword);
        //        var result = await _userManager.UpdateAsync(user);

        //        //var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //        //var result = await _userManager.ResetPasswordAsync(user, token, profileDto.PlainPassword);
        //    }
        //    return NoContent();
        //}

        // DELETE: api/Profiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();


            try
            {
                var profile = await _context.Profiles
    .Include(c => c.AppUser)
    .FirstOrDefaultAsync(c => c.Id == id);
                if (profile == null)
                {
                    return NotFound();
                }

                var applicationForms = await _context.ApplicationForms.Where(c => c.AgentId == id).ToListAsync();
                foreach (var applicationForm in applicationForms)
                    _context.ApplicationForms.Remove(applicationForm);

                _context.Profiles.Remove(profile);
                await _context.SaveChangesAsync();

                if (profile.AppUser != null)
                    await _userManager.DeleteAsync(profile.AppUser);
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return NoContent();
        }

        private bool ProfileExists(Guid id)
        {
            return _context.Profiles.Any(e => e.Id == id);
        }
    }
}
