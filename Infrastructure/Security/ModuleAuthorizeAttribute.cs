using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces;
using SPOT_API.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    /// <summary>
    /// Authorization attribute for module-level permission checks
    /// Usage: [ModuleAuthorize("STOCKS", PermissionType.View)]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ModuleAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _moduleCode;
        private readonly PermissionType _permissionType;

        public ModuleAuthorizeAttribute(string moduleCode, PermissionType permissionType)
        {
            _moduleCode = moduleCode;
            _permissionType = permissionType;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Check if user is authenticated
            var user = context.HttpContext.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get user ID from claims
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get authorization service
            var authService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();
            if (authService == null)
            {
                context.Result = new StatusCodeResult(500); // Internal server error
                return;
            }

            // Check permission
            var hasPermission = await authService.HasModulePermissionAsync(userId, _moduleCode, _permissionType);
            if (!hasPermission)
            {
                context.Result = new ForbidResult(); // 403 Forbidden
                return;
            }
        }
    }
}
