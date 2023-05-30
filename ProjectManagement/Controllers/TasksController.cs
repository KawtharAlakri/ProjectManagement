using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProjectManagement.Models;
using ProjectManagement.ViewModels;

namespace ProjectManagement.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ProjectManagementContext _context;

        public TasksController(ProjectManagementContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index(int? id)
        {
            var projectManagementContext = _context.Tasks.Include(t => t.AssignedToNavigation).Include(t => t.Project).Include(t => t.StatusNavigation).Where(x => x.ProjectId == id);
            ViewBag.projectId = id;
            ViewBag.project_manager = _context.Projects.Find(id).ProjectManager;
            return View(await projectManagementContext.ToListAsync());
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.AssignedToNavigation)
                .Include(t => t.Project)
                .Include(t => t.StatusNavigation)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create(int? id)
         {
            Project project = _context.Projects.Find(id);
            var userProjects = _context.UserProjects.Where(x => x.ProjectId == project.ProjectId);
            List<string> projectMembers = new List<string>();
            foreach (UserProject userProject in userProjects)
            {
                projectMembers.Add(userProject.Username);
            }
            ProjectTaskVM vm = new ProjectTaskVM { project = project, selectedUsers = projectMembers };
            return View(vm);
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectTaskVM vm)
        {
            Models.Task task = vm.task;
            vm.project = _context.Projects.Find(vm.project.ProjectId);
            vm.project.ProjectManagerNavigation = _context.Users.Find(vm.project.ProjectManager);
            vm.project.StatusNavigation = _context.Statuses.Find(vm.project.Status);
            //vm.project = task.Project;
            task.AssignedToNavigation = _context.Users.Find(vm.task.AssignedTo);
            task.Status = _context.Statuses.Find("in progress").StatusName;
            task.StatusNavigation = _context.Statuses.Find("in progress");
            task.CreatedAt = DateTime.Now;
            task.Project = vm.project;
            task.ProjectId = vm.project.ProjectId;
            ModelState.Clear();
            TryValidateModel(task);

            if (ModelState.IsValid)
            {
                //add task
                _context.Add(task);
                await _context.SaveChangesAsync();
                //add notification
                var message = "You have been assigned to a new task: " + task.TaskName;
                var recipient = task.AssignedTo;

               await NotificationsController.PushNotification(recipient, message,_context);

                return RedirectToAction(nameof(Index), new {id = vm.project.ProjectId});
            }

            //populate projectMembers in case it will return the same page 
            var userProjects = _context.UserProjects.Where(x => x.ProjectId == vm.project.ProjectId);
            List<string> projectMembers = new List<string>();
            foreach (UserProject userProject in userProjects)
            {
                projectMembers.Add(userProject.Username);
            }
            vm.selectedUsers = projectMembers;

            return View(vm);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            ViewData["AssignedTo"] = new SelectList(_context.Users, "Username", "Username", task.AssignedTo);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", task.ProjectId);
            ViewData["Status"] = new SelectList(_context.Statuses, "StatusName", "StatusName", task.Status);
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,AssignedTo,TaskName,Description,CreatedAt,DueDate,ProjectId,Status")] Models.Task task)
        {
            if (id != task.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.TaskId))
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
            ViewData["AssignedTo"] = new SelectList(_context.Users, "Username", "Username", task.AssignedTo);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", task.ProjectId);
            ViewData["Status"] = new SelectList(_context.Statuses, "StatusName", "StatusName", task.Status);
            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.AssignedToNavigation)
                .Include(t => t.Project)
                .Include(t => t.StatusNavigation)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'ProjectManagementContext.Tasks'  is null.");
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
          return (_context.Tasks?.Any(e => e.TaskId == id)).GetValueOrDefault();
        }
    }
}
