using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{
    [Authorize]
    public class LogsController : Controller
    {
        private readonly ProjectManagementContext _context;

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

        public async Task<IActionResult> Delete(int? id)
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

        // POST: Logs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Logs == null)
            {
                return Problem("Entity set 'ProjectManagementContext.Logs'  is null.");
            }
            var log = await _context.Logs.FindAsync(id);
            if (log != null)
            {
                _context.Logs.Remove(log);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LogExists(int id)
        {
          return (_context.Logs?.Any(e => e.LogId == id)).GetValueOrDefault();
        }
    }
}
