using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class ProfilePackage : BaseModel
    {
        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        [ForeignKey("Package")]
        public Guid PackageId { get; set; }
        public virtual Package Package { get; set; }

        public string CommisionType { get; set; }
        public int CommisionPercentage { get; set; }

        public float CommisionAmount { get; set; }

        //[NotMapped]
        //public decimal LeaderCommision
        //{
        //    get
        //    {
        //        if (Profile == null)
        //            return -1;
        //        if (Profile.Leader == null)
        //            return -2;
        //        foreach(var leaderProfilePackage in Profile.Leader.ProfilePackages)
        //        {
        //            if (leaderProfilePackage.PackageId == PackageId)
        //                return leaderProfilePackage.CommisionAmount;
        //        }
        //        return -3;
        //    }
        //}

    }
}
