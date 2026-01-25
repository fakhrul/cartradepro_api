using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Models;
using SPOT_API.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SPOT_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserRolesController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuditService _auditService;

        public UserRolesController(
            SpotDBContext context,
            UserManager<AppUser> userManager,
            IAuditService auditService)
        {
            _context = context;
            _userManager = userManager;
            _auditService = auditService;
        }

        /// <summary>
        /// Get all roles assigned to a user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                var userRoles = await _context.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .Include(ur => ur.Role)
                    .OrderByDescending(ur => ur.AssignedAt)
                    .Select(ur => new
                    {
                        ur.Id,
                        ur.RoleId,
                        RoleName = ur.Role.Name,
                        RoleDisplayName = ur.Role.DisplayName,
                        RoleDescription = ur.Role.Description,
                        ur.IsActive,
                        ur.EffectiveFrom,
                        ur.EffectiveUntil,
                        ur.AssignedBy,
                        ur.AssignedAt,
                        IsCurrentlyEffective = ur.IsCurrentlyEffective()
                    })
                    .ToListAsync();

                return Ok(userRoles);
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SystemError,
                    "GetUserRoles",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    $"Error retrieving roles for user: {userId}",
                    "UserRole",
                    userId,
                    AuditSeverity.Medium);
                return StatusCode(500, "Error retrieving user roles");
            }
        }

        /// <summary>
        /// Assign a role to a user
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            try
            {
                // Validate user exists
                var user = await _userManager.FindByIdAsync(dto.UserId);
                if (user == null)
                    return NotFound("User not found");

                // Validate role exists
                var role = await _context.Roles.FindAsync(dto.RoleId);
                if (role == null)
                    return NotFound("Role not found");

                // Check if assignment already exists
                var existingAssignment = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == dto.UserId && ur.RoleId == dto.RoleId && ur.IsActive);

                if (existingAssignment != null)
                    return BadRequest("User already has this role assigned");

                // Get current user ID
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Create new role assignment
                var userRole = new UserRole
                {
                    UserId = dto.UserId,
                    RoleId = dto.RoleId,
                    AssignedBy = currentUserId,
                    AssignedAt = DateTime.UtcNow,
                    EffectiveFrom = dto.EffectiveFrom,
                    EffectiveUntil = dto.EffectiveUntil,
                    IsActive = true
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync(
                    AuditEventType.RoleAssigned,
                    "AssignRole",
                    $"Role '{role.Name}' assigned to user '{user.UserName}'",
                    "UserRole",
                    userRole.Id.ToString(),
                    $"{user.UserName} - {role.Name}",
                    null,
                    new
                    {
                        UserId = dto.UserId,
                        UserName = user.UserName,
                        RoleId = dto.RoleId,
                        RoleName = role.Name,
                        dto.EffectiveFrom,
                        dto.EffectiveUntil
                    },
                    AuditSeverity.Info);

                return Ok(new
                {
                    success = true,
                    userRoleId = userRole.Id,
                    message = $"Role '{role.DisplayName}' successfully assigned to user '{user.DisplayName}'"
                });
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SystemError,
                    "AssignRole",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    "Error assigning role to user",
                    "UserRole",
                    null,
                    AuditSeverity.High);
                return StatusCode(500, "Error assigning role");
            }
        }

        /// <summary>
        /// Revoke a role from a user
        /// </summary>
        [HttpDelete("{userRoleId}")]
        public async Task<IActionResult> RevokeRole(Guid userRoleId)
        {
            try
            {
                var userRole = await _context.UserRoles
                    .Include(ur => ur.User)
                    .Include(ur => ur.Role)
                    .FirstOrDefaultAsync(ur => ur.Id == userRoleId);

                if (userRole == null)
                    return NotFound("Role assignment not found");

                // Prevent removing system roles from SuperAdmin
                if (userRole.Role.IsSystemRole && userRole.Role.Name == "SuperAdmin")
                {
                    var superAdminCount = await _context.UserRoles
                        .Where(ur => ur.Role.Name == "SuperAdmin" && ur.IsActive)
                        .CountAsync();

                    if (superAdminCount <= 1)
                        return BadRequest("Cannot remove the last SuperAdmin role");
                }

                var oldValue = new
                {
                    userRole.UserId,
                    UserName = userRole.User.UserName,
                    userRole.RoleId,
                    RoleName = userRole.Role.Name,
                    userRole.IsActive
                };

                userRole.IsActive = false;
                userRole.EffectiveUntil = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await _auditService.LogAsync(
                    AuditEventType.RoleRevoked,
                    "RevokeRole",
                    $"Role '{userRole.Role.Name}' revoked from user '{userRole.User.UserName}'",
                    "UserRole",
                    userRole.Id.ToString(),
                    $"{userRole.User.UserName} - {userRole.Role.Name}",
                    oldValue,
                    new { IsActive = false, EffectiveUntil = DateTime.UtcNow },
                    AuditSeverity.Medium);

                return Ok(new
                {
                    success = true,
                    message = $"Role '{userRole.Role.DisplayName}' revoked from user '{userRole.User.DisplayName}'"
                });
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SystemError,
                    "RevokeRole",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    $"Error revoking role: {userRoleId}",
                    "UserRole",
                    userRoleId.ToString(),
                    AuditSeverity.High);
                return StatusCode(500, "Error revoking role");
            }
        }

        /// <summary>
        /// Update role assignment (change effective dates or reactivate)
        /// </summary>
        [HttpPut("{userRoleId}")]
        public async Task<IActionResult> UpdateRoleAssignment(Guid userRoleId, [FromBody] UpdateRoleAssignmentDto dto)
        {
            try
            {
                var userRole = await _context.UserRoles
                    .Include(ur => ur.User)
                    .Include(ur => ur.Role)
                    .FirstOrDefaultAsync(ur => ur.Id == userRoleId);

                if (userRole == null)
                    return NotFound("Role assignment not found");

                var oldValue = new
                {
                    userRole.IsActive,
                    userRole.EffectiveFrom,
                    userRole.EffectiveUntil
                };

                // Update fields
                userRole.IsActive = dto.IsActive;
                userRole.EffectiveFrom = dto.EffectiveFrom;
                userRole.EffectiveUntil = dto.EffectiveUntil;

                await _context.SaveChangesAsync();

                await _auditService.LogAsync(
                    AuditEventType.RoleModified,
                    "UpdateRoleAssignment",
                    $"Role assignment updated for user '{userRole.User.UserName}' with role '{userRole.Role.Name}'",
                    "UserRole",
                    userRole.Id.ToString(),
                    $"{userRole.User.UserName} - {userRole.Role.Name}",
                    oldValue,
                    new
                    {
                        dto.IsActive,
                        dto.EffectiveFrom,
                        dto.EffectiveUntil
                    },
                    AuditSeverity.Info);

                return Ok(new
                {
                    success = true,
                    message = "Role assignment updated successfully"
                });
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SystemError,
                    "UpdateRoleAssignment",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    $"Error updating role assignment: {userRoleId}",
                    "UserRole",
                    userRoleId.ToString(),
                    AuditSeverity.High);
                return StatusCode(500, "Error updating role assignment");
            }
        }

        /// <summary>
        /// Get all available roles
        /// </summary>
        [HttpGet("available-roles")]
        public async Task<IActionResult> GetAvailableRoles()
        {
            try
            {
                var roles = await _context.Roles
                    .OrderBy(r => r.Name)
                    .Select(r => new
                    {
                        r.Id,
                        r.Name,
                        r.DisplayName,
                        r.Description,
                        r.IsSystemRole
                    })
                    .ToListAsync();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SystemError,
                    "GetAvailableRoles",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    "Error retrieving available roles",
                    null,
                    null,
                    AuditSeverity.Low);
                return StatusCode(500, "Error retrieving available roles");
            }
        }
    }

    #region DTOs

    public class AssignRoleDto
    {
        public string UserId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveUntil { get; set; }
    }

    public class UpdateRoleAssignmentDto
    {
        public bool IsActive { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveUntil { get; set; }
    }

    #endregion
}
