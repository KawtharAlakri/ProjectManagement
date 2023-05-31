using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
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
            IQueryable<Models.Task> tasksContext = _context.Tasks.Include(t => t.AssignedToNavigation).Include(t => t.Project).Include(t => t.StatusNavigation);
            //display tasks for a project
            if (id.HasValue)
            {
                tasksContext = tasksContext.Where(x => x.ProjectId == id);
                ViewBag.projectId = id;
                ViewBag.project = _context.Projects.Find(id);
                ViewBag.project_manager = _context.Projects.Find(id).ProjectManager;

                //validate that user is member or manager before returning the view 
                var isMember = _context.UserProjects.Any(p => p.ProjectId == id && p.Username == User.Identity.Name);
                if (_context.Projects.Find(id).ProjectManager != User.Identity.Name && !isMember)
                {
                    TempData["ErrorMessage"] = "You do not have permission to view this page.";
                    return RedirectToAction("Index", "Projects");
                }
            }
            return View(await tasksContext.ToListAsync());
        }

        public async Task<IActionResult> MyTasks(string searchString, string statusFilter, int projectFilter)
        {
            //get initial tasks list (all tasks assigned to logged-in user)
            IQueryable<Models.Task> tasksContext = _context.Tasks.Include(t => t.AssignedToNavigation)
                .Include(t => t.Project)
                .Include(t => t.StatusNavigation)
                .Where(t => t.AssignedTo == User.Identity.Name);

            //check for and apply filters 
            if (!String.IsNullOrEmpty(searchString))
            {
                tasksContext = tasksContext.Where(x => x.TaskName.Contains(searchString));
            }
             else if (!String.IsNullOrEmpty(statusFilter))
            {
                tasksContext = tasksContext.Where(x => x.Status == statusFilter);
            }
            else if (projectFilter > 0)
            {
                tasksContext = tasksContext.Where(x => x.ProjectId == projectFilter);
            }

             //get all users' projects 
            var userProjects = _context.Projects
                                   .Include(p => p.ProjectManagerNavigation)
                                   .Include(p => p.StatusNavigation)
                                   .Include(p => p.UserProjects)
                                   .Where(p => p.UserProjects.Any(up => up.Username == User.Identity.Name));

            //pass filtering values in viewBag to keep it once results are shown 
            ViewBag.searchString = searchString;
            ViewData["Status"] = new SelectList(_context.Statuses, "StatusName", "StatusName", statusFilter);
            ViewData["Projects"] = new SelectList(userProjects, "ProjectId", "ProjectName", projectFilter);

            return View(await tasksContext.ToListAsync());
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
                .Include(t => t.Project.UserProjects)
                .Include(t => t.StatusNavigation)
                .Include(t=> t.Comments)
                .Include(t=>t.Documents)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (task == null)
            {
                return NotFound();
            }

            //validate that user is project member or manager before returning task details 
            var isMember = _context.UserProjects.Any(p => p.ProjectId == task.ProjectId && p.Username == User.Identity.Name);
            if (task.Project.ProjectManager != User.Identity.Name && !isMember)
            {
                TempData["ErrorMessage"] = "You do not have permission to view this page.";
                return RedirectToAction("MyTasks", "Tasks");
            }
            ProjectTaskVM vm = new ProjectTaskVM { comments = task.Comments, documents = task.Documents, project = task.Project, task = task};

            return View(vm);
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
            //validate that user is project member or manager before returning create form  
            var isMember = _context.UserProjects.Any(p => p.ProjectId == id && p.Username == User.Identity.Name);
            if (_context.Projects.Find(id).ProjectManager != User.Identity.Name && !isMember)
            {
                TempData["ErrorMessage"] = "You do not have permission to add task in this project.";
                return RedirectToAction("Index", "Projects");
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
                TempData["SuccessMessage"] = "Task Created Successfully.";
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

            Models.Task task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            Project project = _context.Projects.Find(task.ProjectId);
            var userProjects = _context.UserProjects.Where(x => x.ProjectId == project.ProjectId);
            List<string> projectMembers = new List<string>();
            foreach (UserProject userProject in userProjects)
            {
                projectMembers.Add(userProject.Username);
            }
            //validate that user is project member or manager before returning task edit form 
            var isMember = _context.UserProjects.Any(p => p.ProjectId == task.ProjectId && p.Username == User.Identity.Name);
            if (task.Project.ProjectManager != User.Identity.Name && !isMember)
            {
                TempData["ErrorMessage"] = "You do not have permission to edit this task.";
                return RedirectToAction("MyTasks", "Tasks");
            }

            ProjectTaskVM vm = new ProjectTaskVM { project = project, selectedUsers = projectMembers, task = task };
            ViewData["Status"] = new SelectList(_context.Statuses, "StatusName", "StatusName", task.Status);
            return View(vm);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectTaskVM vm)
        {
            Models.Task task = vm.task;
            vm.project = _context.Projects.Find(vm.project.ProjectId);
            vm.project.ProjectManagerNavigation = _context.Users.Find(vm.project.ProjectManager);
            vm.project.StatusNavigation = _context.Statuses.Find(vm.project.Status);
            //vm.project = task.Project;
            task.AssignedToNavigation = _context.Users.Find(vm.task.AssignedTo);
            task.StatusNavigation = _context.Statuses.Find(vm.task.Status);
            task.Project = vm.project;
            task.ProjectId = vm.project.ProjectId;
            ModelState.Clear();
            TryValidateModel(task);

            Project project = vm.project;

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
                TempData["SuccessMessage"] = "Task Updated Successfully.";
                return RedirectToAction(nameof(Index), new {id = project.ProjectId});
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
            //validate that user is project member or manager before returning delete view  
            var isMember = _context.UserProjects.Any(p => p.ProjectId == task.ProjectId && p.Username == User.Identity.Name);
            if (task.Project.ProjectManager != User.Identity.Name && !isMember)
            {
                TempData["ErrorMessage"] = "You do not have permission to delete this task.";
                return RedirectToAction("MyTasks", "Tasks");
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
            var task = await _context.Tasks.Include(t => t.Documents).Include(t=>t.Comments).FirstAsync(t => t.TaskId == id);
            if (task != null)
            {
                //remove all documents 
                 _context.RemoveRange(task.Documents);
                await _context.SaveChangesAsync();

                //remove all comments 
                 _context.RemoveRange(task.Comments);
                await _context.SaveChangesAsync();

                //remove task
                _context.Tasks.Remove(task);
            }
            TempData["SuccessMessage"] = "Task Deleted Successfully.";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new {id = task.ProjectId});
        }

        private bool TaskExists(int id)
        {
          return (_context.Tasks?.Any(e => e.TaskId == id)).GetValueOrDefault();
        }
    }
}
