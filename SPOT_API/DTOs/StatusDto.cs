﻿using SPOT_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.DTOs
{
    public class StatusDto
    {
        public Guid ApplicationFormId { get; set; }

        public string Status { get; set; }

    }
}
