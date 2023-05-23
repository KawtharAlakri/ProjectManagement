using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.ViewModels;

namespace ProjectManagement.Controllers
{
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
            var projectManagementContext = _context.Projects.Include(p => p.ProjectManagerNavigation).Include(p => p.StatusNavigation);
            return View(await projectManagementContext.ToListAsync());
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
                .Include(p => p.StatusNavigation)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
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
            ViewData["ProjectManager"] = new SelectList(_context.Users, "Username", "Username", project.ProjectManager);
            ViewData["Status"] = new SelectList(_context.Statuses, "StatusName", "StatusName", project.Status);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,ProjectName,CreatedAt,DueDate,Budget,Description,ProjectManager,Status")] Project project)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
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
            ViewData["ProjectManager"] = new SelectList(_context.Users, "Username", "Username", project.ProjectManager);
            ViewData["Status"] = new SelectList(_context.Statuses, "StatusName", "StatusName", project.Status);
            return View(project);
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
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
          return (_context.Projects?.Any(e => e.ProjectId == id)).GetValueOrDefault();
        }
    }
}
