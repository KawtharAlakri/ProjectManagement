﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.ViewModels;

namespace ProjectManagement.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ProjectManagementContext _context;

        public CommentsController(ProjectManagementContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index(int? id)
        {
            var projectManagementContext = _context.Comments.Include(c => c.Author).Include(c => c.Task).Where(c=>c.TaskId == id);
            var task = _context.Tasks.Find(id);
            var project = _context.Projects.Include(x => x.ProjectManagerNavigation).FirstOrDefault(x=> x.ProjectId == task.ProjectId);
            TaskDetailsVM vm = new TaskDetailsVM { task = _context.Tasks.Find(id), project = project, comments = await projectManagementContext.ToListAsync() };

            return View(vm);
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
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,CommentText,PostedAt,TaskId,AuthorId")] Comment comment)
        {
            //fill all properties of the comment 
            comment.Author = _context.Users.Find(User.Identity.Name);
            comment.AuthorId = User.Identity.Name;
            comment.PostedAt = DateTime.Now;
            comment.Task = _context.Tasks.Include(t => t.StatusNavigation).Include(t => t.Project).Include(t => t.AssignedToNavigation).FirstOrDefault(t => t.TaskId == comment.TaskId);
            comment.Task.Project = _context.Projects.Include(p => p.ProjectManagerNavigation).Include(p => p.StatusNavigation).FirstOrDefault(p => p.ProjectId == comment.Task.ProjectId);
            
            //re validate 
            ModelState.Clear();
            TryValidateModel(comment);
            
            if (ModelState.IsValid)
            {
                //add comment and return success message
                _context.Add(comment);
                await _context.SaveChangesAsync();

                //log user action
                LogsController.ActionLogChanges(User.Identity.Name, comment, EntityState.Added, ControllerContext, _context);

                TempData["SuccessMessage"] = "Your comment has been added successfully.";
                return RedirectToAction("Index", new { id = comment.TaskId }); //first parameter is the action, then controller, then action parameter
            }

            //return error message 
            TempData["ErrorMessage"] = "An error occurred, try adding your comment again.";
            return RedirectToAction("Index", new { id = comment.TaskId }); //first parameter is the action, then controller, then action parameter
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

            //fill all properties for the comment object 
            comment.Author = _context.Users.Find(comment.AuthorId);
            comment.Task = _context.Tasks.Include(t => t.StatusNavigation).Include(t => t.Project).Include(t => t.AssignedToNavigation).FirstOrDefault(t => t.TaskId == comment.TaskId);
            comment.Task.Project = _context.Projects.Include(p=>p.ProjectManagerNavigation).Include(p=>p.StatusNavigation).FirstOrDefault(p=>p.ProjectId == comment.Task.ProjectId);

            //re validate the model state 
            ModelState.Clear();
            TryValidateModel(comment);
            if (ModelState.IsValid)
            {
                try
                {
                    //update comment 
                    _context.Update(comment);
                    await _context.SaveChangesAsync();

                    //log user action
                    LogsController.ActionLogChanges(User.Identity.Name, comment, EntityState.Modified, ControllerContext, _context);

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
                //redirect to task details (where comments listed) and return success message 
                TempData["SuccessMessage"] = "Your comment has been updated successfully.";
                return RedirectToAction("Index", new { id = comment.TaskId }); //first parameter is the action, then controller, then action parameter
            }

            //stay at the samem page and show error message 
            TempData["ErrorMessage"] = "An error occurred, try updating your comment again.";
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
            TempData["DeleteComment"] = comment.CommentId;
            return RedirectToAction("Index", new { id = comment.TaskId }); //first parameter is the action, then controller, then action parameter
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

            //log user action
            LogsController.ActionLogChanges(User.Identity.Name, comment, EntityState.Deleted, ControllerContext, _context);

            TempData["SuccessMessage"] = "Comment Deleted Successfully.";
            return RedirectToAction("Index", new { id = comment.TaskId }); //first parameter is the action, then controller, then action parameter
        }

        private bool CommentExists(int id)
        {
          return (_context.Comments?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
