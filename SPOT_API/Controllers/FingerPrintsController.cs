using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using SPOT_API.Models;
using SPOT_API.Persistence;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FingerPrintsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly IWebHostEnvironment _env;

        public FingerPrintsController(SpotDBContext context, IUserAccessor userAccessor, IWebHostEnvironment env)
        {
            _context = context;
            _userAccessor = userAccessor;
            _env = env;
        }

        // GET: api/FingerPrints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FingerPrint>>> GetFingerPrint()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            return await _context.FingerPrints
                .Where(c => c.TenantId == user.TenantId)
                .ToListAsync();
        }

        // GET: api/FingerPrints/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FingerPrint>> GetFingerPrint(Guid id)
        {
            var fingerPrint = await _context.FingerPrints.FindAsync(id);

            if (fingerPrint == null)
            {
                return NotFound();
            }

            return fingerPrint;
        }

        // PUT: api/FingerPrints/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFingerPrint(Guid id, FingerPrint fingerPrint)
        {
            if (id != fingerPrint.Id)
            {
                return BadRequest();
            }

            _context.Entry(fingerPrint).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FingerPrintExists(id))
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

        // POST: api/FingerPrints
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FingerPrint>> PostFingerPrint(FingerPrint fingerPrint)
        {
            try
            {
                if (fingerPrint.TenantId == Guid.Empty)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                    fingerPrint.TenantId = user.TenantId.Value;
                }

                _context.FingerPrints.Add(fingerPrint);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetFingerPrint", new { id = fingerPrint.Id }, fingerPrint);

            }
            catch (Exception ex)
            {
            }
            return BadRequest();
        }

        // DELETE: api/FingerPrints/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFingerPrint(Guid id)
        {
            var fingerPrint = await _context.FingerPrints.FindAsync(id);
            if (fingerPrint == null)
            {
                return NotFound();
            }

            _context.FingerPrints.Remove(fingerPrint);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FingerPrintExists(Guid id)
        {
            return _context.FingerPrints.Any(e => e.Id == id);
        }

        [HttpGet("DownloadTemplate")]
        public async Task<FileContentResult> DownloadTemplate()
        {

            var filePath = Path.Combine(_env.ContentRootPath, "Templates") + "\\mlschedule.csv";

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return new FileContentResult(bytes, contentType)
            {
                FileDownloadName = "mlschedule.csv"
            };

        }

        [HttpGet("ProcessFile")]
        public async Task<IActionResult> ProcessFile()
        {
            try
            {

                var folderName = "Uploads";
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (!Directory.Exists(pathToSave))
                    Directory.CreateDirectory(pathToSave);

                var fingerPrints = await _context.FingerPrints.Where(c => c.IsTrainDataGenerated == false).ToListAsync();
                foreach (var fp in fingerPrints)
                {

                    var folder = new DirectoryInfo(pathToSave);

                    var matcher = new Matcher();
                    matcher.AddInclude(fp.DocumentCollectionScheduleId.ToString() + "*");
                    var result = matcher.Execute(new DirectoryInfoWrapper(folder));

                    var file = result.Files.FirstOrDefault();

                    if (file.Path != null)
                    {
                        var filePath = Path.Combine(pathToSave, file.Path);

                        var provider = new FileExtensionContentTypeProvider();
                        if (!provider.TryGetContentType(filePath, out var contentType))
                        {
                            contentType = "application/octet-stream";
                        }
                        using (var streamReader = new StreamReader(filePath))
                        {
                            var header = streamReader.ReadLine();
                            while (!streamReader.EndOfStream)
                            {
                                var line = streamReader.ReadLine();
                                var values = line.Split(',');

                                var deviceId = values[0];
                                var macAddress = values[1];
                                var areaCode = values[2];
                                var startTime = values[3];
                                var endTime = values[4];

                                var area = await _context.Areas.Where(c => c.FingerPrintCode == areaCode).FirstOrDefaultAsync();
                                var device = await _context.Devices.Where(c => c.MacAddress == macAddress).FirstOrDefaultAsync();
                                if (area != null & device != null)
                                {
                                    FingerPrintDetail fpDetail = new FingerPrintDetail()
                                    {
                                        AreaId = area.Id,
                                        AreaCode = area.Code,
                                        Code = areaCode,
                                        FingerPrintId = fp.Id,
                                        DeviceMacAddress = macAddress,
                                        DeviceId = device.Id,
                                        DeviceCode = device.Code,
                                        StartTime = DateTime.ParseExact(startTime, "M/dd/yy HH:mm", null),
                                        EndTime = DateTime.ParseExact(endTime, "M/dd/yy HH:mm", null),

                                    };
                                    await _context.FingerPrintDetails.AddAsync(fpDetail);
                                }
                            }
                        }
                    }
                    fp.IsTrainDataGenerated = true;
                }

                await _context.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

            return NoContent();
        }

    }
}
