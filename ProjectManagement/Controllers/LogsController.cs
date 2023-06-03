using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{
    //[Authorize(Roles ="Admin")]
    public class LogsController : Controller
    {
        private readonly ProjectManagementContext _context = new ProjectManagementContext();

        public LogsController(ProjectManagementContext context)
        {
            _context = context;
        }

        // GET: Logs
        public async Task<IActionResult> Index()
        {
            var projectManagementContext = _context.Logs.Include(l => l.UsernameNavigation);
            return View(await projectManagementContext.ToListAsync());
        }

        public static void ActionLogChanges(string username, object entity, EntityState state, ControllerContext controllerContext, ProjectManagementContext context)
        {
            // Create a new Log object to store the log information
            Log log = new Log
            {
                LogType = "Action",
                LogTimestamp = DateTime.Now,
                Username = username,
                Source = "Web",
                PageSource = controllerContext.HttpContext.Request.Path,
            };

            // Check if the entity is being added or modified
            if (state == EntityState.Added || state == EntityState.Modified)
            {
                var entry = context.Entry(entity);

                // Convert original values to a readable format
                var originalValuesDict = entry.OriginalValues.Properties
                    .ToDictionary(p => p.Name, p => entry.OriginalValues[p]?.ToString());

                // Convert current values to a readable format
                var currentValuesDict = entry.CurrentValues.Properties
                    .ToDictionary(p => p.Name, p => entry.CurrentValues[p]?.ToString());

                // Serialize dictionaries to JSON strings
                log.CurrentValue = JsonConvert.SerializeObject(currentValuesDict);
                log.OriginalValue = JsonConvert.SerializeObject(originalValuesDict);
            }

            // Add the log to the context and save the changes
            context.Logs.Add(log);
            context.SaveChanges();
        }


        // GET: Logs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Logs == null)
            {
                return NotFound();
            }

            var log = await _context.Logs
                .Include(l => l.UsernameNavigation)
                .FirstOrDefaultAsync(m => m.LogId == id);
            if (log == null)
            {
                return NotFound();
            }

            return View(log);
        }

        private bool LogExists(int id)
        {
          return (_context.Logs?.Any(e => e.LogId == id)).GetValueOrDefault();
        }
    }
}
