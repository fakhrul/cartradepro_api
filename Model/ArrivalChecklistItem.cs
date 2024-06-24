using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class ArrivalChecklistItem : BaseModel
    {
        [ForeignKey("ArrivalChecklist")]
        public Guid ArrivalChecklistId { get; set; }
        public virtual ArrivalChecklist ArrivalChecklist { get; set; }

        public string Name { get; set; }

        public bool IsAvailable { get; set; }

        public string Remarks { get; set; }
    }



}
