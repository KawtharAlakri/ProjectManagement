using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.ViewModels;

namespace ProjectManagement.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ProjectManagementContext _context;

        public ProjectsController(ProjectManagementContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            //projects where user is manager 
            var ManagerProjects = _context.Projects.Include(p => p.ProjectManagerNavigation).Include(p => p.StatusNavigation).Where(x => x.ProjectManager == User.Identity.Name);

            //projects where user is member 
            var projectsForMember = from project in _context.Projects
                                    join userProject in _context.UserProjects
                                    on project.ProjectId equals userProject.ProjectId
                                    where userProject.Username == User.Identity.Name
                                    select project;

            // Add Include for related entities in the second query
            projectsForMember = projectsForMember
                .Include(p => p.ProjectManagerNavigation)
                .Include(p => p.StatusNavigation);

            //combine both queries
            var combinedProjects = ManagerProjects.Concat(projectsForMember);

            return View(await combinedProjects.ToListAsync());

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
                .Include(p => p.StatusNavigation).Include(p=>p.Tasks).Include(p=>p.UserProjects)
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
            ProjectUsersVM vm = new ProjectUsersVM { project = project, selectedUsers = selectedUsers};

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

                return RedirectToAction(nameof(Index));
            }

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
            ProjectUsersVM vm = new ProjectUsersVM { project = project,selectedUsers = selectedUsers, allUsers = _context.Users };
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
                    // close the existing DataReader
                    await _context.Database.CloseConnectionAsync();

                    /// update the project
                    _context.Update(viewModel.project);
                    await _context.SaveChangesAsync();

                    

                    //update project members (remove all and insert again) ? 
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
                return RedirectToAction(nameof(Index));
            }
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
            .Include(p => p.Tasks).Include(p=>p.UserProjects)  
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
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
          return (_context.Projects?.Any(e => e.ProjectId == id)).GetValueOrDefault();
        }
    }
}
