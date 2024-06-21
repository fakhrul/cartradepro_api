using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Document : BaseModel
    {
        public string Path { get; set; }

        public string FileName { get; set; }
        public string Driver { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }
}
