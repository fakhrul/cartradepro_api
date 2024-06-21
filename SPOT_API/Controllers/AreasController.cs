using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Persistence;
using SPOT_API.Models;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using Application.Interfaces;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly IWebHostEnvironment _env;

        public AreasController(SpotDBContext context, IUserAccessor userAccessor, IWebHostEnvironment env)
        {
            _context = context;
            _userAccessor = userAccessor;
            _env = env;
        }

        // GET: api/Areas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Area>>> GetAreas()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            return await _context.Areas.Where(c => c.Level.Location.Map.TenantId == user.TenantId)
                .Include(o => o.Level)
                .Include(o => o.Level.Location)
                .Include(o => o.Level.Location.Map)
                .Include(o => o.Level.Location.Map.Tenant)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        // GET: api/Areas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Area>> GetArea(Guid id)
        {
            var area = await _context.Areas
                .Include(c => c.Level)
                .Include(c => c.Level.Location)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (area == null)
            {
                return NotFound();
            }

            return area;
        }

        // PUT: api/Areas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArea(Guid id, Area area)
        {
            if (id != area.Id)
            {
                return BadRequest();
            }

            _context.Entry(area).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaExists(id))
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

        // POST: api/Areas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Area>> PostArea(Area area)
        {
            _context.Areas.Add(area);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArea", new { id = area.Id }, area);
        }

        // DELETE: api/Areas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(Guid id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null)
            {
                return NotFound();
            }

            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AreaExists(Guid id)
        {
            return _context.Areas.Any(e => e.Id == id);
        }


        [HttpPost("UploadFile"), DisableRequestSizeLimit]
        public async Task<ActionResult<IEnumerable<Area>>> Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = "Uploads";
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                    Directory.CreateDirectory(pathToSave);

                List<Area> _areas = new List<Area>();

                if (file.Length > 0)
                {
                    using (var streamReader = new StreamReader(file.OpenReadStream()))
                    {
                        //IDictionary<string, Tenant> _tenantDict = new Dictionary<string, Tenant>();
                        //IDictionary<string, Map> _mapTemp = new Dictionary<string, Map>();
                        var header = streamReader.ReadLine();
                        while (!streamReader.EndOfStream)
                        {
                            var line = streamReader.ReadLine();
                            var values = line.Split(',');
                            var tenantCode = values[0];
                            var tenantObj = _context.Tenants.FirstOrDefault(c => c.Code == tenantCode);
                            if (tenantObj == null)
                            {
                                tenantObj = new Tenant
                                {
                                    Name = tenantCode,
                                    Code = tenantCode,
                                };
                                _context.Tenants.Add(tenantObj);
                            }

                            var mapCode = values[1];
                            var mapName = values[2];
                            var mapObj = _context.Maps.FirstOrDefault(c =>
                                (c.Code == mapCode) && (c.TenantId == tenantObj.Id)
                            );
                            if (mapObj == null)
                            {
                                mapObj = new Map
                                {
                                    TenantId = tenantObj.Id,
                                    Name = mapName,
                                    Code = mapCode,
                                };
                                _context.Maps.Add(mapObj);
                            }

                            var locationCode = values[3];
                            var locationName = values[4];
                            var locationObj = _context.Locations.FirstOrDefault(c =>
                                (c.Code == locationCode) && (c.MapId == mapObj.Id)
                            );
                            if (locationObj == null)
                            {
                                locationObj = new Location
                                {
                                    MapId = mapObj.Id,
                                    Name = locationName,
                                    Code = locationCode,
                                };
                                _context.Locations.Add(locationObj);
                            }

                            var levelCode = values[5];
                            var levelName = values[6];
                            var levelObj = _context.Levels.FirstOrDefault(c =>
                                (c.Code == levelCode) && (c.LocationId == locationObj.Id)
                            );
                            if (levelObj == null)
                            {
                                levelObj = new Level
                                {
                                    LocationId = locationObj.Id,
                                    Name = levelName,
                                    Code = levelCode,
                                };
                                _context.Levels.Add(levelObj);
                            }

                            var areaCode = values[7];
                            var areaName = values[8];
                            var areaFingerPrint = values[9];
                            var areaX = values[10];
                            var areaY = values[11];

                            var areaObj = _context.Areas.FirstOrDefault(c =>
                                (c.Code == areaCode) && (c.LevelId == levelObj.Id)
                            );
                            if (areaObj == null)
                            {
                                areaObj = new Area
                                {
                                    LevelId = levelObj.Id,
                                    Name = areaName,
                                    Code = areaCode,
                                    FingerPrintCode = areaFingerPrint,
                                    ImageCoordX = int.Parse(areaX),
                                    ImageCoordY = int.Parse(areaY),
                                };
                                _context.Areas.Add(areaObj);
                                _areas.Add(areaObj);
                            }
                            await _context.SaveChangesAsync();
                        }


                    }
                    //var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    //var fullPath = Path.Combine(pathToSave, fileName);
                    //var dbPath = Path.Combine(folderName, fileName);

                    //using (var stream = new FileStream(fullPath, FileMode.Create))
                    //{
                    //    file.CopyTo(stream);
                    //}

                    return _areas;
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }


        [HttpGet("DownloadFileTemplate")]
        public async Task<FileContentResult> DownloadFileTemplate()
        {

            var filePath = Path.Combine(_env.ContentRootPath, "Templates") + "\\area.csv";

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return new FileContentResult(bytes, contentType)
            {
                FileDownloadName = "area.csv"
            };

        }

        [HttpGet("ByLevel/{id}")]
        public async Task<ActionResult<IEnumerable<Area>>> GetAreaByLevel(Guid id)
        {
            return await _context.Areas.Where(c => c.LevelId == id).ToListAsync();

        }

    }
}
