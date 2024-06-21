using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Controllers
{
    public class HelloController : Controller
    {
        public IActionResult Index()
        {
            //   var timeZones = TimeZoneInfo.GetSystemTimeZones();
            //return new ApiException("asdas");

            return Content("API v1.0.0.8 - Server Date Time is " + DateTime.Now );
            //return View();
        }
    }
}
