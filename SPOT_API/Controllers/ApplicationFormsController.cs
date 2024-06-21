using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.DTOs;
using SPOT_API.Models;
using SPOT_API.Persistence;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationFormsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;

        public ApplicationFormsController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
        }

        // GET: api/ApplicationForms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationForm>>> GetApplicationForms()
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                DateTime dtStart = DateTime.Now.AddMonths(-1).ToUniversalTime();
                DateTime dtEnd = DateTime.Now.ToUniversalTime();
                var objs = await _context.ApplicationForms
                    .Include(c => c.Agent)
                    .Include(c => c.Agent.ProfilePackages)
                    //.Include(c => c.ServiceProvider)
                    .Include(c => c.Package)
                    .Include(c => c.RemarksList)
                    .Where(c => c.SubmittedDate >= dtStart && c.SubmittedDate <= dtEnd)
                    .OrderByDescending(c => c.SubmittedDate)
                    .ToListAsync();

                foreach (var obj in objs)
                {
                    obj.RemarksList = null;
                    obj.RemarksList = null;
                    if (obj.Agent != null)
                    {
                        obj.Agent.AppUser = null;
                        foreach (var profilePackage in obj.Agent.ProfilePackages)
                        {
                            profilePackage.Profile = null;
                            profilePackage.Package = null;
                        }
                    }
                    if (obj.CreatedBy != null)
                        obj.CreatedBy.AppUser = null;
                    //obj.State = "Selangor";
                }

                return objs;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet("ByFilter/{start}/{end}")]
        public async Task<ActionResult<IEnumerable<ApplicationForm>>> GetApplicationForms(string start, string end)
        {

            try
            {
                DateTime dtStart = DateTime.ParseExact(start, "yyyyMMddHHmm", null).ToUniversalTime();
                DateTime dtEnd = DateTime.ParseExact(end, "yyyyMMddHHmm", null).ToUniversalTime();

                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                var objs = new List<ApplicationForm>();

                if (user.Role.ToLower() == "super")
                {
                    objs = await _context.ApplicationForms
                        .Include(c => c.Agent)
                        .Include(c => c.Agent.ProfilePackages)
                        //.Include(c => c.ServiceProvider)
                        .Include(c => c.Package)
                        .Include(c => c.RemarksList)
                        .Where(c => c.SubmittedDate >= dtStart && c.SubmittedDate <= dtEnd)
                        .OrderByDescending(c => c.SubmittedDate)
                        .ToListAsync();
                }
                else
                {
                    objs = await _context.ApplicationForms
                        .Include(c => c.Agent)
                        .Include(c => c.Agent.ProfilePackages)
                        //.Include(c => c.ServiceProvider)
                        .Include(c => c.Package)
                        .Include(c => c.RemarksList)
                        .Where(c => c.SubmittedDate >= dtStart && c.SubmittedDate <= dtEnd)
                        .Where(c => c.AgentId == user.ProfileId || c.Agent.LeaderId == user.ProfileId)
                        .OrderByDescending(c => c.SubmittedDate)
                        .ToListAsync();

                }

                foreach (var obj in objs)
                {
                    obj.RemarksList = null;
                    if (obj.Agent != null)
                    {
                        obj.Agent.AppUser = null;
                        foreach (var profilePackage in obj.Agent.ProfilePackages)
                        {
                            profilePackage.Profile = null;
                            profilePackage.Package = null;
                        }
                    }
                    if (obj.CreatedBy != null)
                        obj.CreatedBy.AppUser = null;


                }

                return objs;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        // GET: api/ApplicationForms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationForm>> GetApplicationForm(Guid id)
        {
            var applicationForm = await _context.ApplicationForms
                .Include(c => c.Agent)
                .Include(c => c.CreatedBy)
                .Include(c => c.RemarksList.OrderByDescending(d => d.DateTime))
                .Include(c => c.ApplicationDocumentList)
                .Include("ApplicationDocumentList.Document")
                .Include("RemarksList.Profile")
                .FirstOrDefaultAsync(c => c.Id == id);

            if (applicationForm == null)
            {
                return NotFound();
            }

            foreach (var item in applicationForm.RemarksList)
            {
                //if (item.ApplicationForm != null)
                //    item.ApplicationForm = null;
            }

            foreach (var item in applicationForm.ApplicationDocumentList)
            {
                if (item.ApplicationForm != null)
                    item.ApplicationForm = null;
                if (item.Document != null)
                    item.Document.Content = null;
            }

            return applicationForm;
        }

        [HttpGet("DownloadDocument/{id}")]
        public async Task<ActionResult> GetApplicationFormDownloadDocument(Guid id)
        {
            var documents = await _context.ApplicationDocuments
                .Include(c => c.Document)
                .Where(c => c.ApplicationFormId == id)
                .ToListAsync();

            var zipName = $"file-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";

            //var files = Directory.GetFiles(Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory)).ToList();

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var document in documents)
                    {
                        var theFile = archive.CreateEntry(document.Document.FileName);
                        using (var entryStream = theFile.Open())
                        {
                            using (var streamWriter = new StreamWriter(entryStream))
                            {
                                streamWriter.Write(document.Document.Content);
                            }
                        }

                        //using (var streamWriter = new StreamWriter(theFile.Open()))
                        //{
                        //    using (MemoryStream ms = new MemoryStream(document.Document.Content))
                        //    {
                        //        streamWriter.Write(ms.ToArray());
                        //    }
                        //        //streamWriter.Write(document.Document.Content);
                        //    //using (MemoryStream ms = new MemoryStream(file.Document.Content))
                        //    //{
                        //    //    streamWriter.Write(ms.ToArray());
                        //    //}

                        //}

                    }
                    //documents.ForEach(file =>
                    //{
                    //    var theFile = archive.CreateEntry(file.Document.FileName);
                    //    using (var streamWriter = new StreamWriter(theFile.Open()))
                    //    {
                    //        using (MemoryStream ms = new MemoryStream(file.Document.Content))
                    //        {
                    //            streamWriter.Write(ms.ToArray());
                    //        }

                    //    }

                    //});
                }

                using (var fileStream = new FileStream(@"C:\Temp\test.zip", FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(ms);

                    return File(ms.ToArray(), "application/zip", zipName);
                    //return ms.ToArray();
                }


                //return File(memoryStream.ToArray(), "application/zip", zipName);
            }
            return NotFound();


            //var applicationForm = await _context.ApplicationForms
            //    .Include(c => c.Agent)
            //    .Include(c => c.CreatedBy)
            //    .Include(c => c.RemarksList.OrderByDescending(d => d.DateTime))
            //    .Include(c => c.ApplicationDocumentList)
            //    .Include("ApplicationDocumentList.Document")
            //    .Include("RemarksList.Profile")
            //    .FirstOrDefaultAsync(c => c.Id == id);

            //if (applicationForm == null)
            //{
            //    return NotFound();
            //}

            //foreach (var item in applicationForm.RemarksList)
            //{
            //    if (item.ApplicationForm != null)
            //        item.ApplicationForm = null;
            //}

            //foreach (var item in applicationForm.ApplicationDocumentList)
            //{
            //    if (item.ApplicationForm != null)
            //        item.ApplicationForm = null;
            //    if (item.Document != null)
            //        item.Document.Content = null;
            //}

            //return applicationForm;
        }


        [HttpPut("RemoveDocument/{applicationFormId}/{documentId}")]
        public async Task<IActionResult> PutApplicationFormRemoveDocument(Guid applicationFormId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var applicationDocuments = _context.ApplicationDocuments
                    .Where(c => c.ApplicationFormId == applicationFormId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in applicationDocuments)
                {
                    _context.ApplicationDocuments.Remove(inDataBase);
                }

                var documents = _context.Documents
                    .Where(c => c.Id == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.Documents.Remove(inDataBase);
                }

                await _context.SaveChangesAsync();

            }

            catch (Exception ex)
            {
                throw ex;

            }





            return NoContent();
        }

        // PUT: api/ApplicationForms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicationForm(Guid id, ApplicationForm applicationForm)
        {
            if (id != applicationForm.Id)
            {
                return BadRequest();
            }


            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var previousApplicationDocuments = _context.ApplicationDocuments.Where(c => c.ApplicationFormId == applicationForm.Id).ToList();
                foreach (var inDataBase in previousApplicationDocuments)
                {
                    _context.ApplicationDocuments.Remove(inDataBase);
                }
                //_context.SaveChanges();

                if (applicationForm.IsOwnApplication)
                    if ((applicationForm.AgentId == null) ||
                        (applicationForm.AgentId != applicationForm.CreatedById))
                        applicationForm.AgentId = applicationForm.CreatedById;

                IList<ApplicationDocument> docs = new List<ApplicationDocument>();
                foreach (var item in applicationForm.ApplicationDocumentList)
                {
                    //if (item.ApplicationFormId == null || item.ApplicationFormId == Guid.Empty)
                    //    item.ApplicationFormId = applicationForm.Id;
                    ApplicationDocument doc = new ApplicationDocument
                    {
                        ApplicationFormId = applicationForm.Id,
                        DocumentId = item.DocumentId,
                    };

                    docs.Add(doc);
                    //    item.Id = Guid.Empty;
                    //
                    //}
                }
                _context.ApplicationDocuments.AddRange(docs);
                //_context.SaveChanges();
                await _context.SaveChangesAsync();


                _context.Entry(applicationForm).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationFormExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            catch (Exception ex)
            {
                throw ex;

            }





            return NoContent();
        }

        [HttpPut("AddRemarks/{id}")]
        public async Task<IActionResult> PutApplicationFormAddRemarks(Guid id, Remarks remarks)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var applicationForm = await _context.ApplicationForms
                .FirstOrDefaultAsync(c => c.Id == id);


            if (applicationForm == null)
            {
                return BadRequest();
            }

            if (remarks.ProfileId == Guid.Empty)
                remarks.ProfileId = user.ProfileId.Value;

            if (applicationForm.RemarksList == null)
                applicationForm.RemarksList = new List<Remarks>();
            applicationForm.RemarksList.Add(remarks);

            _context.Remarks.Add(remarks);
            await _context.SaveChangesAsync();

            _context.Entry(applicationForm).State = EntityState.Modified;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationFormExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            catch (Exception ex)
            {
                throw ex;

            }
            return NoContent();
        }


        [HttpPut("UpdateOfficialOrderNo/{id}")]
        public async Task<IActionResult> PutApplicationFormUpdateOfficialOrderNo(Guid id, ApplicationForm form)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var applicationForm = await _context.ApplicationForms
                .FirstOrDefaultAsync(c => c.Id == id);


            if (applicationForm == null)
            {
                return BadRequest();
            }

            applicationForm.ProviderOrderNo = form.ProviderOrderNo;

            _context.Entry(applicationForm).State = EntityState.Modified;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationFormExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            catch (Exception ex)
            {
                throw ex;

            }
            return NoContent();
        }


        [HttpPut("UpdatePaymentStatus/{id}")]
        public async Task<IActionResult> PutApplicationFormUpdatePaymentStatus(Guid id, ApplicationForm form)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var applicationForm = await _context.ApplicationForms
                .FirstOrDefaultAsync(c => c.Id == id);


            if (applicationForm == null)
            {
                return BadRequest();
            }

            applicationForm.IsPaid = form.IsPaid;
            applicationForm.PaymentDate = form.PaymentDate;

            _context.Entry(applicationForm).State = EntityState.Modified;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationFormExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            catch (Exception ex)
            {
                throw ex;

            }
            return NoContent();
        }

        [HttpPut("UpdateInternalStatus/{id}")]
        public async Task<IActionResult> PutApplicationFormUpdateInternalStatus(Guid id, StatusDto status)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var applicationForm = await _context.ApplicationForms
                .FirstOrDefaultAsync(c => c.Id == id);


            if (applicationForm == null)
            {
                return BadRequest();
            }

            applicationForm.InternalStatus = status.Status;

            _context.Entry(applicationForm).State = EntityState.Modified;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationFormExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            catch (Exception ex)
            {
                throw ex;

            }
            return NoContent();
        }

        [HttpPut("UpdateProviderStatus/{id}")]
        public async Task<IActionResult> PutApplicationFormUpdateProviderStatus(Guid id, StatusDto status)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var applicationForm = await _context.ApplicationForms
                .FirstOrDefaultAsync(c => c.Id == id);


            if (applicationForm == null)
            {
                return BadRequest();
            }

            applicationForm.ProviderStatus = status.Status;

            _context.Entry(applicationForm).State = EntityState.Modified;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationFormExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            catch (Exception ex)
            {
                throw ex;

            }
            return NoContent();
        }

        // POST: api/ApplicationForms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApplicationForm>> PostApplicationForm(ApplicationForm applicationForm)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                if (applicationForm.CreatedById == null || applicationForm.CreatedById == Guid.Empty)
                    applicationForm.CreatedById = user.ProfileId.Value;

                if (applicationForm.IsOwnApplication)
                    applicationForm.AgentId = applicationForm.CreatedById;

                foreach (var applicationDocument in applicationForm.ApplicationDocumentList)
                {
                    applicationDocument.ApplicationForm = null;
                    applicationDocument.Document = null;
                }

                _context.ApplicationForms.Add(applicationForm);
                await _context.SaveChangesAsync();


                foreach (var item in applicationForm.RemarksList)
                {
                    //if (item.ApplicationForm != null)
                    //    item.ApplicationForm = null;
                }

                foreach (var item in applicationForm.ApplicationDocumentList)
                {
                    if (item.ApplicationForm != null)
                        item.ApplicationForm = null;
                    if (item.Document != null)
                        item.Document.Content = null;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return CreatedAtAction("GetApplicationForm", new { id = applicationForm.Id }, applicationForm);
        }

        // DELETE: api/ApplicationForms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicationForm(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();



            var applicationForm = await _context.ApplicationForms
                .FirstOrDefaultAsync(c => c.Id == id);
            if (applicationForm == null)
            {
                return NotFound();
            }

            _context.ApplicationForms.Remove(applicationForm);
            await _context.SaveChangesAsync();


            return NoContent();
        }

        private bool ApplicationFormExists(Guid id)
        {
            return _context.ApplicationForms.Any(e => e.Id == id);
        }
    }
}
