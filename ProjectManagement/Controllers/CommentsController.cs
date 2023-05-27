using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ProjectManagementContext _context;

        public CommentsController(ProjectManagementContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var projectManagementContext = _context.Comments.Include(c => c.Author).Include(c => c.Task);
            return View(await projectManagementContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Task)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            //ViewData["AuthorId"] = new SelectList(_context.Users, "Username", "Username");
            //ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,CommentText,PostedAt,TaskId,AuthorId")] Comment comment)
        {
            //fill the comment object
            comment.Author = _context.Users.Find(User.Identity.Name);
            comment.AuthorId = User.Identity.Name;
            comment.PostedAt = DateTime.Now;
            comment.Task = _context.Tasks.Find(comment.TaskId);

            //fill the task object  
            comment.Task.Project = _context.Projects.Find(comment.Task.ProjectId);
            comment.Task.AssignedToNavigation = _context.Users.Find(comment.Task.AssignedTo);
            comment.Task.StatusNavigation = _context.Statuses.Find(comment.Task.Status);
            comment.Author.Tasks = _context.Tasks.Where(x => x.AssignedTo == comment.AuthorId).ToList();

            //fill project object 
            comment.Task.Project.ProjectManagerNavigation = _context.Users.Find(comment.Task.Project.ProjectManager);
            comment.Task.Project.StatusNavigation = _context.Statuses.Find(comment.Task.Project.Status);
            ModelState.Clear();
            TryValidateModel(comment);
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Your comment has been added successfully.";
                return RedirectToAction("Details", "Tasks", new { id = comment.TaskId }); //first parameter is the action, then controller, then action parameter
            }
            TempData["ErrorMessage"] = "An error occurred, try adding your comment again.";
            return RedirectToAction("Details", "Tasks", new { id = comment.TaskId }); //first parameter is the action, then controller, then action parameter
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Username", "Username", comment.AuthorId);
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", comment.TaskId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,CommentText,PostedAt,TaskId,AuthorId")] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
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
            ViewData["AuthorId"] = new SelectList(_context.Users, "Username", "Username", comment.AuthorId);
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", comment.TaskId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Task)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set 'ProjectManagementContext.Comments'  is null.");
            }
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
          return (_context.Comments?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
