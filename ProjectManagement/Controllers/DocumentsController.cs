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
        public async Task<IActionResult> Index(int? taskid)
        {
            IQueryable<Document> documentsContext = _context.Documents.Include(d => d.Task).Include(d => d.UploadedByNavigation);
            if (taskid != null)
            {
                ViewBag.Task = _context.Tasks.Find(taskid);
                documentsContext = documentsContext.Where(d => d.TaskId == taskid);
            } 
            return View(await documentsContext.ToListAsync());
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

        // GET: Documents/Create
        public IActionResult Create(int? taskid)
        {
            //pass task id to create a document related to it
            ViewBag.TaskId = taskid;
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentVM documentVM)
        {
            if (ModelState.IsValid)
            {

                Document document = new Document();
                document.TaskId = documentVM.TaskId;
                document.DocumentName = documentVM.DocumentName;
                document.DocumentType = documentVM.DocumentType;
                document.UploadedAt = DateTime.Now;
                document.UploadedBy = User.Identity.Name;
                // Get the current directory path
                string currentDirectory = Directory.GetCurrentDirectory();

                // Combine the current directory with the "Uploads" folder
                string uploadsFolder = Path.Combine(currentDirectory, "Uploads");

                // Generate a unique file name for the uploaded file
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + documentVM.uploadedFile.FileName;

                // Construct the file path
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the uploaded file to the file system
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await documentVM.uploadedFile.CopyToAsync(fileStream);
                }

                // Store the file path in the document entity
                document.FilePath = filePath;
                _context.Add(document);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Document Uploaded Successfully.";
                return RedirectToAction("Index", new {taskid = document.TaskId});
                
            }
            
            return View(documentVM);
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            //you're still not passing the file path (that is already there)
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DocumentId,DocumentName,FilePath,DocumentType,UploadedAt,TaskId,UploadedBy")] Document document)
        {
            if (id != document.DocumentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(document);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.DocumentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", document.TaskId);
            ViewData["UploadedBy"] = new SelectList(_context.Users, "Username", "Username", document.UploadedBy);
            return View(document);
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
                _context.Documents.Remove(document);
            }
            
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Document Deleted Successfully.";
            return RedirectToAction(nameof(Index), new { taskid = document.TaskId });
        }

        private bool DocumentExists(int id)
        {
          return (_context.Documents?.Any(e => e.DocumentId == id)).GetValueOrDefault();
        }
    }
}
