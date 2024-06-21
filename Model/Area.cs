using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Area : BaseModel
    {
        [ForeignKey("Level")]
        public Guid LevelId { get; set; }

        public virtual Level Level { get; set; }

        //public string LevelName
        //{
        //    get
        //    {
        //        return Level.Name;
        //    }
        //}
        public string Name { get; set; }
        public string Code { get; set; }

        public string FingerPrintCode { get; set; }

        public int ImageCoordX { get; set; }

        public int ImageCoordY { get; set; }

        public Guid? DocumentMapId { get; set; }
        [ForeignKey("Document")]
        public virtual Document DocumentMap { get; set; }

        public int Count { get; set; }
        public DateTime CountLastUpdated { get; set; }

    }
}
