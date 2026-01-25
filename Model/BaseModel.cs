using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class BaseModel
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Created On TimeStamp
        /// </summary>

        [Required]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// UpdatedOn TimeStamp
        /// </summary>
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User who created this record (string to match AppUser.Id from IdentityUser)
        /// </summary>
        [JsonIgnore]
        public string CreatedBy { get; set; }

        /// <summary>
        /// User who last updated this record (string to match AppUser.Id from IdentityUser)
        /// </summary>
        [JsonIgnore]
        public string UpdatedBy { get; set; }

        /// <summary>
        /// User who deleted this record (for soft deletes) (string to match AppUser.Id from IdentityUser)
        /// </summary>
        [JsonIgnore]
        public string DeletedBy { get; set; }

        public BaseModel()
        {
            if (Id == Guid.Empty)
                Id = Guid.NewGuid();
        }
    }
}
