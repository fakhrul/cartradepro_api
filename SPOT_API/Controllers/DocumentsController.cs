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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.AspNetCore.StaticFiles;
using SPOT_API.DTOs;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IConfiguration _config;

        public DocumentsController(SpotDBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: api/Documents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Document>>> GetDocument()
        {
            return await _context.Documents.ToListAsync();
        }

        // GET: api/Documents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> GetDocument(Guid id)
        {
            var document = await _context.Documents.FindAsync(id);

            if (document == null)
            {
                return NotFound();
            }

            return document;
        }

        // PUT: api/Documents/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocument(Guid id, Document document)
        {
            if (id != document.Id)
            {
                return BadRequest();
            }

            _context.Entry(document).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentExists(id))
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

        // POST: api/Documents
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Document>> PostDocument(Document document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDocument", new { id = document.Id }, document);
        }

        // DELETE: api/Documents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(Guid id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DocumentExists(Guid id)
        {
            return _context.Documents.Any(e => e.Id == id);
        }
        //[HttpPost("UploadImage"), DisableRequestSizeLimit]
        //public async Task<ActionResult<IEnumerable<DocumentDto>>> UploadImage()
        //{
        //    try
        //    {
        //        var azureBlobConnection = _config.GetConnectionString("SpotFileStore");
        //        List<DocumentDto> _imagesUrl = new List<DocumentDto>();
        //        foreach (var file in Request.Form.Files)
        //        {

        //            if (file.Length > 0)
        //            {
        //                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //                var extension = Path.GetExtension(fileName).Replace(".", "");

        //                Guid fileId = Guid.NewGuid();
                        
        //                var fullName = fileId + "." + extension;
        //                var container = new BlobContainerClient(azureBlobConnection, "spot-dashboard");
        //                var createResponse = await container.CreateIfNotExistsAsync();
        //                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
        //                    await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

        //                var blob = container.GetBlobClient(fullName);
        //                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        //                using (var fileStream = file.OpenReadStream())
        //                {
        //                    await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = file.ContentType });
        //                }

        //                Document doc = new Document
        //                {
        //                    Id = fileId,
        //                    Path = blob.Uri.ToString(),
        //                    FileName = fullName,
        //                    //Driver = "upload"
        //                };
        //                _context.Documents.Add(doc);

        //                _imagesUrl.Add(new DocumentDto
        //                {
        //                    Id = fileId,
        //                    Url = _config["ApiUrl"] + "/Documents/Image/" + fileId.ToString()
        //                });
        //                //levelObj.DocumentMapId = fileId;
        //                await _context.SaveChangesAsync();
        //                //}
        //            }
        //        }

        //        return _imagesUrl;
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex}");
        //    }
        //}

        //[HttpPost("UploadImage"), DisableRequestSizeLimit]
        //public async Task<ActionResult<IEnumerable<DocumentDto>>> UploadImage()
        //{
        //    try
        //    {
        //        var folderName = "Uploads";
        //        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //        if (!Directory.Exists(pathToSave))
        //            Directory.CreateDirectory(pathToSave);

        //        List<DocumentDto> _imagesUrl = new List<DocumentDto>();

        //        foreach (var file in Request.Form.Files)
        //        {

        //            if (file.Length > 0)
        //            {
        //                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //                var extension = Path.GetExtension(fileName).Replace(".", "");

        //                //var levelCode = fileName.Split('.')[0];
        //                //var extension = fileName.Split('.')[1];

        //                //var levelObj = _context.Levels.FirstOrDefault(c => (c.Code == levelCode));
        //                //if (levelObj != null)
        //                //{
        //                Guid fileId = Guid.NewGuid();
        //                var fullPath = Path.Combine(pathToSave, fileId + "." + extension);
        //                using (var stream = new FileStream(fullPath, FileMode.Create))
        //                {
        //                    file.CopyTo(stream);
        //                }

        //                Document doc = new Document
        //                {
        //                    Id = fileId,
        //                    Path = fullPath,
        //                    Driver = "local"
        //                };
        //                _context.Documents.Add(doc);

        //                _imagesUrl.Add(new DocumentDto{
        //                     Id = fileId,
        //                     Url = _config["ApiUrl"] + "/Documents/Image/" + fileId.ToString() });
        //                //levelObj.DocumentMapId = fileId;
        //                await _context.SaveChangesAsync();
        //                //}
        //            }
        //        }

        //        return _imagesUrl;
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex}");
        //    }
        //}

        [HttpGet("Image/{id}")]
        public async Task<IActionResult> GetImage(Guid id)
        {
            var azureBlobConnection = _config.GetConnectionString("SpotFileStore");
            var container = new BlobContainerClient(azureBlobConnection, "spot-dashboard");
            var createResponse = await container.CreateIfNotExistsAsync();
            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            var blobClient = container.GetBlobClient(document.FileName);


            var downloadContent = await blobClient.DownloadAsync();
            using (MemoryStream ms = new MemoryStream())
            {
                await downloadContent.Value.Content.CopyToAsync(ms);
                string contentType = downloadContent.Value.Details.ContentType;
                return File(ms.ToArray(), contentType, document.FileName);
                //return ms.ToArray();
            }

            //var provider = new FileExtensionContentTypeProvider();
            //    if (!provider.TryGetContentType(filePath, out var contentType))
            //    {
            //        contentType = "application/octet-stream";
            //    }
            //    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

            //    return File(bytes, contentType);


            return NotFound();

        }

        [HttpGet("File/{id}")]
        public async Task<IActionResult> GetFile(Guid id)
        {

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            // Set default content type if not set
            if (string.IsNullOrEmpty(document.ContentType))
                document.ContentType = "application/octet-stream";

            if (document.Content != null)
            {
                using (MemoryStream ms = new MemoryStream(document.Content))
                {
                    // Check if the content type is PDF to serve inline
                    if (document.ContentType == "application/pdf")
                    {
                        Response.Headers.Add("Content-Disposition", $"inline; filename={document.FileName}");
                    }
                    else
                    {
                        Response.Headers.Add("Content-Disposition", $"attachment; filename={document.FileName}");
                    }

                    return File(ms.ToArray(), document.ContentType);
                }
            }

            return NotFound();

            //var document = await _context.Documents.FindAsync(id);
            //if (document == null)
            //{
            //    return NotFound();
            //}
            //if (string.IsNullOrEmpty(document.ContentType))
            //    document.ContentType = "application/octet-stream";

            //using (MemoryStream ms = new MemoryStream(document.Content))
            //{
            //     return File(ms.ToArray(), document.ContentType, document.FileName);
            //    //return ms.ToArray();
            //}

            //return NotFound();

        }

        //[HttpPost("UploadFile"), DisableRequestSizeLimit]
        //public async Task<ActionResult<IEnumerable<DocumentDto>>> UploadFile()
        //{
        //    try
        //    {
        //        var azureBlobConnection = _config.GetConnectionString("SpotFileStore");
        //        List<DocumentDto> _imagesUrl = new List<DocumentDto>();

        //        foreach (var file in Request.Form.Files)
        //        {

        //            if (file.Length > 0)
        //            {
        //                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //                var extension = Path.GetExtension(fileName).Replace(".", "");

        //                Guid fileId = Guid.NewGuid();
        //                var fullName = fileId + "." + extension;
        //                var container = new BlobContainerClient(azureBlobConnection, "spot-dashboard");
        //                var createResponse = await container.CreateIfNotExistsAsync();
        //                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
        //                    await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

        //                var blob = container.GetBlobClient(fullName);
        //                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        //                using (var fileStream = file.OpenReadStream())
        //                {
        //                    await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = file.ContentType });
        //                }

        //                Document doc = new Document
        //                {
        //                    Id = fileId,
        //                    Path = blob.Uri.ToString(),
        //                    FileName = fullName,
        //                    //Driver = "upload"
        //                };
        //                _context.Documents.Add(doc);

        //                _imagesUrl.Add(new DocumentDto
        //                {
        //                    Id = fileId,
        //                    Url = _config["ApiUrl"] + "/Documents/File/" + fileId.ToString()
        //                });
        //                //levelObj.DocumentMapId = fileId;
        //                await _context.SaveChangesAsync();
        //                //}
        //            }
        //        }

        //        return _imagesUrl;

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex}");
        //    }
        //}



        [HttpPost("UploadFile"), DisableRequestSizeLimit]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> UploadFile()
        {
            try
            {
                //var folderName = "Uploads";
  //              var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
    //            if (!Directory.Exists(pathToSave))
      //              Directory.CreateDirectory(pathToSave);

                List<DocumentDto> documentUrls = new List<DocumentDto>();

                foreach (var file in Request.Form.Files)
                {

                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var extension = Path.GetExtension(fileName).Replace(".", "");

                        Document doc = new Document();
                        doc.FileName = fileName;
                        doc.Extension = extension;
                        doc.Driver = "db";

                        var provider = new FileExtensionContentTypeProvider();
                        string contentType;
                        if (!provider.TryGetContentType(fileName, out contentType))
                        {
                            contentType = "application/octet-stream";
                        }
                        doc.ContentType = contentType;

                        using (var stream = new MemoryStream())
                        {
                            file.CopyTo(stream);
                            doc.Content = stream.ToArray();
                        }

                        _context.Documents.Add(doc);
                        await _context.SaveChangesAsync();

                        documentUrls.Add(new DocumentDto
                        {
                            Id = doc.Id,
                            Url = "/Documents/File/" + doc.Id.ToString(),
                            FileName = doc.FileName,
                        });
                        //levelObj.DocumentMapId = fileId;


                        //Guid fileId = Guid.NewGuid();
                        //var fullPath = Path.Combine(pathToSave, fileId + "." + extension);
                        //using (var stream = new FileStream(fullPath, FileMode.Create))
                        //{
                        //    file.CopyTo(stream);
                        //}

                        //Document doc = new Document
                        //{
                        //    Id = fileId,
                        //    Path = fullPath,
                        //    Driver = "local"
                        //};
                        //_context.Documents.Add(doc);

                        //documentUrls.Add(new DocumentDto
                        //{
                        //    Id = fileId,
                        //    Url = _config["ApiUrl"] + "/Documents/File/" + fileId.ToString()
                        //});
                        ////levelObj.DocumentMapId = fileId;
                        //await _context.SaveChangesAsync();
                        //}
                    }
                }

                return documentUrls;

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost("UploadImage"), DisableRequestSizeLimit]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> UploadImage()
        {
            try
            {
                List<DocumentDto> documentUrls = new List<DocumentDto>();

                foreach (var file in Request.Form.Files)
                {
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var extension = Path.GetExtension(fileName).Replace(".", "");

                        Document doc = new Document
                        {
                            FileName = fileName,
                            Extension = extension,
                            Driver = "db"
                        };

                        // Determine content type
                        var provider = new FileExtensionContentTypeProvider();
                        if (!provider.TryGetContentType(fileName, out string contentType))
                        {
                            contentType = "application/octet-stream";
                        }
                        doc.ContentType = contentType;

                        // Progress handler for tracking upload progress
                        var progressHandler = new Progress<long>(progress =>
                        {
                            // Calculate progress percentage
                            var percentage = (double)progress / file.Length * 100;
                            Console.WriteLine($"Progress: {percentage}%"); // Log progress
                                                                           // Optionally, implement SignalR or another mechanism to send real-time progress updates to the client
                        });

                        using (var stream = new MemoryStream())
                        {
                            // Copy to memory stream with progress
                            await CopyToMemoryStreamWithProgressAsync(file, stream, progressHandler);

                            doc.Content = stream.ToArray();
                        }

                        _context.Documents.Add(doc);
                        await _context.SaveChangesAsync();

                        documentUrls.Add(new DocumentDto
                        {
                            Id = doc.Id,
                            Url = "/Documents/File/" + doc.Id.ToString(),
                            FileName = doc.FileName,
                        });
                    }
                }

                return documentUrls;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Helper method to copy file content to MemoryStream with progress tracking
        private async Task CopyToMemoryStreamWithProgressAsync(IFormFile file, MemoryStream stream, IProgress<long> progress)
        {
            byte[] buffer = new byte[81920]; // 80 KB buffer size
            long totalRead = 0;
            int bytesRead;
            using (var inputStream = file.OpenReadStream())
            {
                while ((bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    await stream.WriteAsync(buffer, 0, bytesRead);
                    totalRead += bytesRead;
                    progress?.Report(totalRead); // Report progress
                }
            }
        }


        //[HttpPost("UploadImage"), DisableRequestSizeLimit]
        //public async Task<ActionResult<IEnumerable<DocumentDto>>> UploadImage()
        //{
        //    try
        //    {
        //        //var folderName = "Uploads";
        //        //              var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //        //            if (!Directory.Exists(pathToSave))
        //        //              Directory.CreateDirectory(pathToSave);

        //        List<DocumentDto> documentUrls = new List<DocumentDto>();

        //        foreach (var file in Request.Form.Files)
        //        {

        //            if (file.Length > 0)
        //            {
        //                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //                var extension = Path.GetExtension(fileName).Replace(".", "");

        //                Document doc = new Document();
        //                doc.FileName = fileName;
        //                doc.Extension = extension;
        //                doc.Driver = "db";

        //                var provider = new FileExtensionContentTypeProvider();
        //                string contentType;
        //                if (!provider.TryGetContentType(fileName, out contentType))
        //                {
        //                    contentType = "application/octet-stream";
        //                }
        //                doc.ContentType = contentType;

        //                using (var stream = new MemoryStream())
        //                {
        //                    file.CopyTo(stream);
        //                    doc.Content = stream.ToArray();
        //                }

        //                _context.Documents.Add(doc);
        //                await _context.SaveChangesAsync();

        //                documentUrls.Add(new DocumentDto
        //                {
        //                    Id = doc.Id,
        //                    Url = "/Documents/File/" + doc.Id.ToString(),
        //                    FileName = doc.FileName,
        //                });
        //                //levelObj.DocumentMapId = fileId;


        //                //Guid fileId = Guid.NewGuid();
        //                //var fullPath = Path.Combine(pathToSave, fileId + "." + extension);
        //                //using (var stream = new FileStream(fullPath, FileMode.Create))
        //                //{
        //                //    file.CopyTo(stream);
        //                //}

        //                //Document doc = new Document
        //                //{
        //                //    Id = fileId,
        //                //    Path = fullPath,
        //                //    Driver = "local"
        //                //};
        //                //_context.Documents.Add(doc);

        //                //documentUrls.Add(new DocumentDto
        //                //{
        //                //    Id = fileId,
        //                //    Url = _config["ApiUrl"] + "/Documents/File/" + fileId.ToString()
        //                //});
        //                ////levelObj.DocumentMapId = fileId;
        //                //await _context.SaveChangesAsync();
        //                //}
        //            }
        //        }

        //        return documentUrls;

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex}");
        //    }
        //}

    }
}
