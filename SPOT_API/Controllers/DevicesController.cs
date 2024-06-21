using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Persistence;
using SPOT_API.Models;
using Application.Interfaces;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public DevicesController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/Devices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();
            if (user.IsSuperAdmin)
                return Unauthorized();

            var devices = await _context.Devices
                .Where(c => c.Tenant.Id == user.TenantId)
                .OrderBy(c => c.Name)
                .Include(c => c.DeviceType)
                .Include(c => c.LocationLog)
                .ToListAsync();

            foreach (var dev in devices)
                if (dev.LocationLog != null && dev.LocationLog.Device != null)
                    dev.LocationLog.Device = null;
            return devices;
        }


        [HttpGet("ByProfileNone")]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevicesByProfileNone()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();
            if (user.IsSuperAdmin)
                return Unauthorized();

            return await _context.Devices
                .Where(c => c.Tenant.Id == user.TenantId && c.ProfileId == null)
                .OrderBy(c => c.Name)
                .Include(c => c.DeviceType)
                .ToListAsync();
        }

        // GET: api/Devices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> GetDevice(Guid id)
        {
            var device = await _context.Devices
                .Include(c => c.DeviceType)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }


        [HttpGet("ByCode/{code}")]
        public async Task<ActionResult<Device>> GetDeviceByCode(string code)
        {
            var device = await _context.Devices
                .Include(c => c.DeviceType)
                .FirstOrDefaultAsync(c => c.Code == code);

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }


        [HttpGet("StatusByCode/{code}")]
        public async Task<ActionResult<string>> GetDeviceStatusByCode(string code)
        {
            var device = await _context.Devices
                .Include(c => c.DeviceType)
                .FirstOrDefaultAsync(c => c.Code == code);

            if (device == null)
            {
                return "Not Found";
            }

            if (device.ProfileId == null || device.ProfileId == Guid.Empty)
                return "Available";

            //if (device.ProfileId == Guid.Empty)
            //    return "Available";

            return "Already Assigned";
        }


        // PUT: api/Devices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevice(Guid id, Device device)
        {
            if (id != device.Id)
            {
                return BadRequest();
            }

            _context.Entry(device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Devices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Device>> PostDevice(Device device)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

            device.TenantId = user.TenantId.Value;

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDevice", new { id = device.Id }, device);
        }

        // DELETE: api/Devices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(Guid id)
        {
            try
            {
                var device = await _context.Devices.FindAsync(id);
                if (device == null)
                {
                    return NotFound();
                }

                _context.Devices.Remove(device);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {

                throw;
            }

            return NoContent();
        }

        private bool DeviceExists(Guid id)
        {
            return _context.Devices.Any(e => e.Id == id);
        }
    }
}
