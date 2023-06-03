using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using ProjectManagement.Models;
using ProjectManagement.ViewModels;

namespace ProjectManagement.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly ProjectManagementContext _context;

        public DocumentsController(ProjectManagementContext context)
        {
            _context = context;
        }

        // GET: Documents
        public async Task<IActionResult> Index(int? id)
        {
            IQueryable<Document> documentsContext = _context.Documents.Include(d => d.Task).Include(d => d.UploadedByNavigation);
            DocumentDetailsVM vm = new DocumentDetailsVM();
            if (id != null)
            {
                var task = _context.Tasks.Include(x=> x.Project).FirstOrDefault(x=>x.TaskId == id);
                documentsContext = documentsContext.Where(d => d.TaskId == id);
                vm.documents = documentsContext;
                vm.task = task;
            }
            return View(vm);
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.Task)
                .Include(d => d.UploadedByNavigation)
                .FirstOrDefaultAsync(m => m.DocumentId == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        public IActionResult GetFile(string fileName)
        {
            // Get the file extension
            string fileExtension = Path.GetExtension(fileName).ToLower();

            // Set the MIME type based on the file extension
            string mimeType;
            switch (fileExtension)
            {
                case ".pdf":
                    mimeType = "application/pdf";
                    break;
                case ".doc":
                case ".docx":
                    mimeType = "application/msword";
                    break;
                case ".xls":
                case ".xlsx":
                    mimeType = "application/vnd.ms-excel";
                    break;
                case ".ppt":
                case ".pptx":
                    mimeType = "application/vnd.ms-powerpoint";
                    break;
                default:
                    mimeType = "application/octet-stream"; // Default MIME type for unknown file types
                    break;
            }

            return File(fileName, mimeType, Path.GetFileName(fileName));
        }


        public IActionResult GetImage(string fileName)
        {
            // Construct the full file path
            string filePath = Path.Combine("wwwroot", fileName);

            // Read the file bytes
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            // Determine the content type based on the file extension
            string contentType = GetContentType(filePath);

            // Return the file as a FileResult
            return File(fileBytes, contentType);
        }

        private string GetContentType(string filePath)
        {
            // Determine the content type based on the file extension
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            string contentType = "application/octet-stream"; // Default content type

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
            }

            return contentType;
        }

        // GET: Documents/Create
        public IActionResult Create(int? id)
        {
            //pass task id to create a document related to it
            ViewBag.TaskId = id;
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentVM documentVM, [FromServices] IWebHostEnvironment hostingEnvironment)
        {
            if (ModelState.IsValid)
            {
                Document document = new Document();
                document.TaskId = documentVM.taskid;
                document.Task = _context.Tasks.Find(document.TaskId);
                document.DocumentName = documentVM.DocumentName;
                document.UploadedAt = DateTime.Now;
                document.UploadedBy = User.Identity.Name;

                // Generate a unique file name for the uploaded file
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + documentVM.uploadedFile.FileName;

                // Combine the uploads folder path with the unique file name
                string uploadsFolder = Path.Combine("Uploads", uniqueFileName);

                // Construct the file path relative to the wwwroot folder
                string filePath = Path.Combine(hostingEnvironment.WebRootPath, uploadsFolder);

                // Save the uploaded file to the file system
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await documentVM.uploadedFile.CopyToAsync(fileStream);
                }

                // Store the relative file path in the document entity
                document.FilePath = uploadsFolder;

                // Get the file extension from the uploaded file
                string fileExtension = Path.GetExtension(documentVM.uploadedFile.FileName).ToLower();

                // Determine the document type based on the file extension
                if (IsImageFile(fileExtension))
                {
                    document.DocumentType = "Photo";
                }
                else
                {
                    document.DocumentType = "File";
                }

                    _context.Add(document);
                await _context.SaveChangesAsync();

                //log user action
                LogsController.ActionLogChanges(User.Identity.Name, document, EntityState.Added, ControllerContext, _context);

                TempData["SuccessMessage"] = "Document Uploaded Successfully.";
                return RedirectToAction("Index", new { id = document.TaskId });
            }

            return View(documentVM);
        }

        private bool IsImageFile(string fileExtension)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            return imageExtensions.Contains(fileExtension);
        }
     

     
        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.Task)
                .Include(d => d.UploadedByNavigation)
                .FirstOrDefaultAsync(m => m.DocumentId == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Documents == null)
            {
                return Problem("Entity set 'ProjectManagementContext.Documents'  is null.");
            }
            var document = await _context.Documents.FindAsync(id);
            if (document != null)
            {
                //remove and log user action
                _context.Documents.Remove(document);
                LogsController.ActionLogChanges(User.Identity.Name, document, EntityState.Deleted, ControllerContext, _context);
                await _context.SaveChangesAsync();

                // Get the file path of the document
                var filePath = Path.Combine("wwwroot", document.FilePath);

                // Check if the file exists and delete it from filesystem 
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }


                TempData["SuccessMessage"] = "Document Deleted Successfully.";
            }            
            return RedirectToAction(nameof(Index), new { id = document.TaskId });
        }

        private bool DocumentExists(int id)
        {
          return (_context.Documents?.Any(e => e.DocumentId == id)).GetValueOrDefault();
        }
    }
}
