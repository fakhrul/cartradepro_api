using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SPOT_API.Models;
using SPOT_API.Persistence;
using SPOT_API.Repository;

// Reference: https://techbrij.com/asp-net-core-postgresql-dapper-crud

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceLogsController : ControllerBase
    {
        //private readonly SpotDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly DeviceLogRepository _deviceLogRepo;

        public DeviceLogsController(IConfiguration configuration)
        {
            //_context = context;
            _configuration = configuration;

            if (_deviceLogRepo == null)
                _deviceLogRepo = new DeviceLogRepository(_configuration);

        }

        // GET: api/DeviceLogs
        [HttpGet]
        public async Task<IEnumerable<DeviceLog>> GetDeviceLog()
        {
            return _deviceLogRepo.FindAll();
        }

        // GET: api/DeviceLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceLog>> GetDeviceLog(int id)
        {
            var deviceLog = _deviceLogRepo.FindByID(id);

            if (deviceLog == null)
            {
                return NotFound();
            }

            return deviceLog;
        }

        // PUT: api/DeviceLogs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeviceLog(int id, DeviceLog deviceLog)
        {
            if (id != deviceLog.Id)
            {
                return BadRequest();
            }

            DeviceLog obj = _deviceLogRepo.FindByID(id);
            if (obj == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _deviceLogRepo.Update(obj);
                return RedirectToAction("Index");
            }


            return NoContent();

            //_context.Entry(deviceLog).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!DeviceLogExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return NoContent();
        }

        // POST: api/DeviceLogs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DeviceLog>> PostDeviceLog(DeviceLog deviceLog)
        {

            if (deviceLog.createdate == DateTime.MinValue)
                deviceLog.createdate = DateTime.Now;

            DateTimeOffset dto = new DateTimeOffset(DateTime.Now.AddHours(8));
            deviceLog.date_time = dto.ToUnixTimeSeconds().ToString();

            if (ModelState.IsValid)
            {
                _deviceLogRepo.Add(deviceLog);
            }

            //_context.DeviceLog.Add(deviceLog);
            //await _context.SaveChangesAsync();

            return CreatedAtAction("GetDeviceLog", new { id = deviceLog.Id }, deviceLog);
        }

        // DELETE: api/DeviceLogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeviceLog(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _deviceLogRepo.Remove(id);

            return NoContent();
        }

        //private bool DeviceLogExists(int id)
        //{
        //    return _context.DeviceLog.Any(e => e.Id == id);
        //}
    }
}
