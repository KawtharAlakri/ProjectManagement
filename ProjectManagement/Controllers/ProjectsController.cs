using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NuGet.Packaging.Signing;
using ProjectManagement.Hubs;
using ProjectManagement.Models;
using ProjectManagement.ViewModels;

namespace ProjectManagement.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ProjectManagementContext _context;
        private readonly IHubContext<NotificationsHub> _hubContext;
        public ProjectsController(ProjectManagementContext context, IHubContext<NotificationsHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: Projects
        public async Task<IActionResult> Index(string searchString, string filterBy, string statusFilter)
        {

            //get the full project context with projects that user is manager of or a member in 
            IQueryable<Project> fullProjectsContext = _context.Projects
                                   .Include(p => p.ProjectManagerNavigation)
                                   .Include(p => p.StatusNavigation)
                                   .Include(p => p.UserProjects)
                                   .Where(p => p.UserProjects.Any(up => up.Username == User.Identity.Name) || p.ProjectManager == User.Identity.Name);

            //apply any search or filter 
            if (!String.IsNullOrEmpty(searchString))
            {
                fullProjectsContext = fullProjectsContext.Where(p => p.ProjectName.Contains(searchString));
            }
            if (filterBy == "member")
            {
                fullProjectsContext = fullProjectsContext.Where(p => p.UserProjects.Any(up => up.Username == User.Identity.Name));
            }
            else if (filterBy == "manager")
            {
                fullProjectsContext = fullProjectsContext.Where(p => p.ProjectManager == User.Identity.Name);

            }
            else if (!String.IsNullOrEmpty(statusFilter))
            {
                fullProjectsContext = fullProjectsContext.Where(p => p.Status == statusFilter);
            }

            //pass filtering values in viewBag to keep it once results are shown 
            ViewBag.searchString = searchString;
            ViewBag.filterBy = filterBy;
            ViewData["Status"] = new SelectList(_context.Statuses, "StatusName", "StatusName", statusFilter);

            return View(await fullProjectsContext.ToListAsync());

        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.ProjectManagerNavigation)
                .Include(p => p.StatusNavigation).Include(p => p.Tasks).Include(p => p.UserProjects)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            var isMember = _context.UserProjects.Any(p => p.Project == project && p.Username == User.Identity.Name);
            if (project.ProjectManager != User.Identity.Name && !isMember)
            {
                TempData["ErrorMessage"] = "You do not have permission to view this project.";
                return RedirectToAction(nameof(Index));
            }
            var userProjects = _context.UserProjects.Where(x => x.ProjectId == project.ProjectId);
            List<string> selectedUsers = new List<string>();
            foreach (UserProject userProject in userProjects)
            {
                selectedUsers.Add(userProject.Username);
            }
            ProjectUsersVM vm = new ProjectUsersVM { project = project, selectedUsers = selectedUsers };

            return View(vm);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            ViewBag.Users = new SelectList(_context.Users, "Username", "Username");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectUsersVM viewModel)
        {
            // Create the project
            Project project = viewModel.project;
            project.CreatedAt = DateTime.Now;
            project.ProjectManagerNavigation = _context.Users.Where(x => x.Username == User.Identity.Name).FirstOrDefault();
            project.ProjectManager = User.Identity.Name;
            project.Status = _context.Statuses.FirstOrDefault(s => s.StatusName == "in progress")?.StatusName;
            project.StatusNavigation = _context.Statuses.FirstOrDefault(s => s.StatusName == "in progress");

            ModelState.Clear();
            TryValidateModel(project);
            if (ModelState.IsValid)
            {
                _context.Projects.Add(project);
                //log user action
                LogsController.ActionLogChanges(User.Identity.Name, project, EntityState.Added, ControllerContext, _context);
                await _context.SaveChangesAsync();
                // Add users to the UserProject table
                if (viewModel.selectedUsers != null && viewModel.selectedUsers.Count() > 0)
                {
                    foreach (string username in viewModel.selectedUsers)
                    {
                        User user = _context.Users.FirstOrDefault(u => u.Username == username);
                        if (user != null)
                        {
                            UserProject userProject = new UserProject
                            {
                                ProjectId = project.ProjectId,
                                Username = user.Username
                            };
                            _context.UserProjects.Add(userProject);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Project Created Successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Sorry, an error occurred, try again.";
            return View(viewModel);
        }


        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            if (project.ProjectManager != User.Identity.Name)
            {
                TempData["ErrorMessage"] = "You do not have permission to edit this project.";
                return RedirectToAction(nameof(Index));
            }

            var userProjects = _context.UserProjects.Where(x => x.ProjectId == project.ProjectId);
            List<string> selectedUsers = new List<string>();
            foreach (UserProject userProject in userProjects)
            {
                selectedUsers.Add(userProject.Username);
            }
            ProjectUsersVM vm = new ProjectUsersVM { project = project, selectedUsers = selectedUsers, allUsers = _context.Users };
            ViewData["Status"] = new SelectList(_context.Statuses, "StatusName", "StatusName", project.Status);

            return View(vm);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectUsersVM viewModel)
        {

            viewModel.allUsers = _context.Users;
            viewModel.project.ProjectManagerNavigation = _context.Users.Where(x => x.Username == viewModel.project.ProjectManager).FirstOrDefault();
            viewModel.project.StatusNavigation = _context.Statuses.Where(x => x.StatusName == viewModel.project.Status).FirstOrDefault();

            ModelState.Clear();
            TryValidateModel(viewModel);

            if (ModelState.IsValid)
            {
                try
                {
                    // close any existing DataReader
                    await _context.Database.CloseConnectionAsync();

                    // Check if the project status has changed
                    var previousStatus = await _context.Projects.Where(p => p.ProjectId == viewModel.project.ProjectId).Select(p => p.Status).FirstOrDefaultAsync();

                    /// update the project
                    _context.Update(viewModel.project);

                    //log user action
                    LogsController.ActionLogChanges(User.Identity.Name, viewModel.project, EntityState.Modified, ControllerContext, _context);

                    await _context.SaveChangesAsync();


                    // Check if the project status has changed
                    var currentStatus = viewModel.project.Status;
                   // if (previousStatus != currentStatus)
                   // {
                   //     var message = $"The status of project {viewModel.project.ProjectName} has changed to {currentStatus}";

                   //     // Get the ApplicationUser object for the project manager
                   //     var recipient = viewModel.project.ProjectManager.ToString();

                   //     // Save the notification to the database and broadcast to all clients
                   //     await NotificationsController.PushNotification(recipient, message, _context, _hubContext);
                   // }
                   //// Broadcast the notification to all clients using SignalR
                   //                    var notifications = new List<Notification>
                   //{
                   //     new Notification { NotificationText = $"The status of project '{viewModel.project.ProjectName}' has changed to '{currentStatus}'" }
                   //};
                   // await _hubContext.Clients.All.SendAsync("getUpdatedNotifications", notifications);

               // }//this used to work

                //    if (previousStatus != currentStatus)
                //{
                //    var message = $"The status of project {viewModel.project.ProjectName} has changed to {currentStatus}";
                //    var recipient = viewModel.project.ProjectManager.ToString();
                //    await NotificationsController.PushNotification2(recipient, message, _context);

                //}
                if (previousStatus != currentStatus)
                {
                        var message = $"The status of project' {viewModel.project.ProjectName}' has changed to '{currentStatus}'";
                        var recipient = viewModel.project.ProjectManager.ToString();
                        await NotificationsController.PushNotification2(recipient, message, _context);
                        var notifications2 = new List<Notification>
                {
                    new Notification { NotificationText = message }
                };
                    await _hubContext.Clients.All.SendAsync("getUpdatedNotifications", notifications2);
                }

                //update project members (remove all and insert again) 
                var records = _context.UserProjects.Where(p => p.ProjectId == viewModel.project.ProjectId);
                    _context.UserProjects.RemoveRange(records);
                    //insert 
                    if (viewModel.selectedUsers != null && viewModel.selectedUsers.Count() > 0)
                    {
                        foreach (string username in viewModel.selectedUsers)
                        {
                            User user = _context.Users.FirstOrDefault(u => u.Username == username);
                            if (user != null)
                            {
                                UserProject userProject = new UserProject
                                {
                                    ProjectId = viewModel.project.ProjectId,
                                    Username = user.Username
                                };
                                _context.UserProjects.Add(userProject);
                            }
                        }


                        await _context.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(viewModel.project.ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Project Updated Successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Sorry, an error occurred, try again.";
            ViewData["Status"] = new SelectList(_context.Statuses, "StatusName", "StatusName", viewModel.project.Status);
            return View(viewModel);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }


            var project = await _context.Projects
                .Include(p => p.ProjectManagerNavigation)
                .Include(p => p.StatusNavigation)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }
            if (project.ProjectManager != User.Identity.Name)
            {
                TempData["ErrorMessage"] = "You do not have permission to delete this project.";
                return RedirectToAction(nameof(Index));
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'ProjectManagementContext.Projects'  is null.");
            }
            var project = await _context.Projects
            .Include(p => p.Tasks).Include(p => p.UserProjects)
            .SingleOrDefaultAsync(p => p.ProjectId == id);
            if (project != null)
            {
                //delete userProjects records 
                _context.RemoveRange(project.UserProjects);
                await _context.SaveChangesAsync();

                //remove project tasks
                _context.RemoveRange(project.Tasks);
                await _context.SaveChangesAsync();

                //delete the project itself 
                _context.Projects.Remove(project);
                
                //log user action
                LogsController.ActionLogChanges(User.Identity.Name, project, EntityState.Deleted, ControllerContext, _context);
                
                await _context.SaveChangesAsync();

                
            }
            TempData["SuccessMessage"] = "Project Deleted Successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.ProjectId == id)).GetValueOrDefault();
        }
        // get the dashboard view
        //public async Task<IActionResult> View(int id)
        //{
        //    var project = await _context.Projects
        //        .Include(p => p.Tasks)
        //        .FirstOrDefaultAsync(p => p.ProjectId == id);

        //    if (project == null)
        //    {
        //        return NotFound();
        //    }

        //    var allUsers = await _context.Users.ToListAsync();

        //    var selectedUsers = await _context.UserProjects
        //        .Where(up => up.ProjectId == id)
        //        .Select(up => up.Username)
        //        .ToListAsync();

        //    var model = new ProjectUsersVM
        //    {
        //        project = project,
        //        allUsers = allUsers,
        //        selectedUsers = selectedUsers
        //    };

        //    return View(model);
        //}

        //tester
        public async Task<IActionResult> View(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            if (project.Tasks == null || project.Tasks.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "This project has no tasks.");
            }

            var allUsers = await _context.Users.ToListAsync();

            var selectedUsers = await _context.UserProjects
                .Where(up => up.ProjectId == id)
                .Select(up => up.Username)
                .ToListAsync();

            var model = new ProjectUsersVM
            {
                project = project,
                allUsers = allUsers,
                selectedUsers = selectedUsers
            };

            return View(model);
        }


        //public IActionResult pendingTask()
        //{
        //    try
        //    {
        //        var overdueTaskCount = _context.Projects
        //            .SelectMany(p => p.Tasks)
        //            .Count(t => t.Status == "overdue");
        //        return Json(overdueTaskCount);
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }
        //}

        public IActionResult pendingTask(int projectId)
        {
            try
            {
                var taskCounts = _context.Projects
                    .Where(p => p.ProjectId == projectId)
                    .SelectMany(p => p.Tasks)
                    .GroupBy(t => t.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToDictionary(x => x.Status, x => x.Count);
                return Json(taskCounts);
            }
            catch
            {
                return BadRequest();
            }
        }

        public IActionResult GetTaskCountsByMember(int projectId)
        {
            try
            {
                var taskCounts = _context.Projects
                    .Where(p => p.ProjectId == projectId)
                    .SelectMany(p => p.Tasks)
                    .GroupBy(t => t.AssignedTo)
                    .Select(g => new
                    {
                        AssignedTo = g.Key,
                        Count = g.Count()
                    })
                    .ToDictionary(x => x.AssignedTo, x => x.Count);

                return Json(taskCounts);
            }
            catch
            {
                return BadRequest();
            }
        }
        public IActionResult DaysLeft(int projectId)
        {
            try
            {
                var project = _context.Projects
                    .SingleOrDefault(p => p.ProjectId == projectId);

                if (project == null)
                {
                    return NotFound();
                }

                if (project.Status == "completed")
                {
                    return Json(new { DaysLeft = "Completed project" });
                }

                int? daysLeft = null;

                if (project.DueDate.HasValue)
                {
                    daysLeft = (project.DueDate.Value - DateTime.Today).Days;
                }

                return Json(new { DaysLeft = daysLeft });
            }
            catch
            {
                return BadRequest();
            }
        }
        //public IActionResult getProjectTasksStats(int projectId)
        //{
        //    var _project = _context.Projects.FirstOrDefault(p=>p.ProjectId == projectId);

        //}

    }
}
