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
    }
}
