using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.DTOs
{
    public class RegisterDto
    {
        //public string DisplayName { get; set; }
        //public string Email { get; set; }
        //public string Password { get; set; }
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        //[Required]
        //[RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$", ErrorMessage = "Password must be complex")]
        //public string Password { get; set; }

        //[Required]
        public string UserName { get; set; }
        //[Required]
        public string TenantCode { get; set; }

        public Guid DepartmentId { get; set; }

        public string Role { get; set; }
        public string IdNo { get; set; }
        public string Phone { get; set; }

    }
}
