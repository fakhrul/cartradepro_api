using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.DTOs
{
    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }

        public string FileName { get; set; }
    }
}
