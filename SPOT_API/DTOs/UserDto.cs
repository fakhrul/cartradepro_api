﻿using SPOT_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.DTOs
{
    public class UserDto
    {
        public string DisplayName { get; set; }

        public string Token { get; set; }

        public string UserName { get; set; }

        public string Image{ get; set; }

        public bool IsSuperAdmin { get; set; }
        public string Role { get; set; }
        public string TenantCode { get; set; }

        public Guid? TenantId { get; set; }

        public bool IsAzureAd { get; set; }
        public string[] Roles { get; internal set; }
        //public List<RoleModulePermission> Permisions { get; internal set; }
        public List<PermissionDto> Permisions { get; internal set; }

        //public Dictionary<string, List<string>> Modules { get; internal set; }
        public List<ModuleRoleDto> Modules { get; set; }
    }

    public class ModuleRoleDto
    {
        public string Module { get; set; }
        public List<string> Roles { get; set; }
    }
    public class PermissionDto
    {
        public string Module { get; set; }
        public bool CanAdd { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }
    }
}
