using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Persistence;
using SPOT_API.Models;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.AspNetCore.StaticFiles;
using Application.Interfaces;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public LocationsController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/Locations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            return await _context.Locations.Where(c => c.Map.TenantId == user.TenantId)
                .Include(p => p.Map).ToListAsync();

        }

        // GET: api/Locations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Location>> GetLocation(Guid id)
        {
            var location = await _context.Locations.FindAsync(id);

            if (location == null)
            {
                return NotFound();
            }

            return location;
        }

        [HttpGet("ByMap/{id}")]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocationByMap(Guid id)
        {
            return await _context.Locations.Where(c=> c.MapId == id).ToListAsync();

        }

        [HttpGet("ByTenant/{id}")]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocationByTenant(Guid id)
        {
            return await _context.Locations
                .Where(c => c.Map.TenantId == id)
                .OrderBy(c=> c.Name)
                .ToListAsync();

        }

        // PUT: api/Locations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocation(Guid id, Location location)
        {
            if (id != location.Id)
            {
                return BadRequest();
            }

            _context.Entry(location).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(id))
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

        // POST: api/Locations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Location>> PostLocation(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocation", new { id = location.Id }, location);
        }

        // DELETE: api/Locations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(Guid id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocationExists(Guid id)
        {
            return _context.Locations.Any(e => e.Id == id);
        }

        [HttpPost("UploadFloorPlan"), DisableRequestSizeLimit]
        public async Task<ActionResult<IEnumerable<string>>> Upload()
        {
            try
            {
                var folderName = "Uploads";
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (!Directory.Exists(pathToSave))
                    Directory.CreateDirectory(pathToSave);

                List<string> _imagesUrl = new List<string>();

                foreach (var file in Request.Form.Files)
                {

                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var levelCode = fileName.Split('.')[0];
                        var extension = fileName.Split('.')[1];

                        var levelObj = _context.Levels.FirstOrDefault(c => (c.Code == levelCode));
                        if (levelObj != null)
                        {
                            Guid fileId = Guid.NewGuid();
                            var fullPath = Path.Combine(pathToSave, fileId + "." + extension);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            Document doc = new Document
                            {
                                Id = fileId,
                                Path = fullPath,
                                Driver = "local"
                            };
                            _context.Documents.Add(doc);
                            //_config["ApiUrl"] + "/Documents/Image/" + fileId.ToString()

                            _imagesUrl.Add("https://localhost:44305/api/Locations/FloorPlan/" +fileId.ToString());
                            levelObj.DocumentMapId = fileId;
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                return _imagesUrl;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("FloorPlan/{id}")]
        public async Task<IActionResult> GetFloorPlan(Guid id)
        {
            var folderName = "Uploads";
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(pathToSave);

            var folder = new DirectoryInfo(pathToSave);

            var matcher = new Matcher();
            matcher.AddInclude(id.ToString() + "*");
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
                var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                return File(bytes, contentType);
            }


            return NotFound();

        }

    }
}
