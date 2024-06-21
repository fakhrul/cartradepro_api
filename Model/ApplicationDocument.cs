using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class ApplicationDocument : BaseModel
    {
        [ForeignKey("ApplicationForm")]
        public Guid ApplicationFormId { get; set; }
        public virtual ApplicationForm ApplicationForm { get; set; }

        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }
    }
}
