﻿using SPOT_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.DTOs
{
    public class StockStatusDto
    {
        public Guid StockId { get; set; }
        public Guid StockStatusId { get; set; }

    }
    
}
