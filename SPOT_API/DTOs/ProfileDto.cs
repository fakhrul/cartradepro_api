using SPOT_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.DTOs
{
    public class ProfileDto
    {
        public Profile Profile { get; set; }

        public bool IsResetPassword { get; set; }

        //public bool IsAzureAd { get; set; }

        public string PlainPassword { get; set; }
        public int DefaultCommisionPercentage { get; set; }

    }
}
