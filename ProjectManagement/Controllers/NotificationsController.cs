using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Hubs;
using ProjectManagement.Models;
using ProjectManagement.ViewModels;


namespace ProjectManagement.Controllers
{

    public class NotificationsController : Controller
    {
        private readonly ProjectManagementContext _context;
        private readonly NotificationsHub _hubcontext;

        public NotificationsController(ProjectManagementContext context, NotificationsHub hubcontext)
        {
            _context = context;
            _hubcontext = hubcontext;
        }


        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            return View(await _context.Notifications.ToListAsync());
        }
        // GET: Notifications
        //public async Task<IActionResult> Index()
        //{
        //    var projectManagementContext = _context.Notifications.Include(n => n.RecipientNavigation);
        //    return View(await projectManagementContext.ToListAsync());
        //}

        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.RecipientNavigation)
                .FirstOrDefaultAsync(m => m.NotificationId == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // GET: Notifications/Create
        public IActionResult Create()
        {
            ViewData["Recipient"] = new SelectList(_context.Users, "Username", "Username");
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notification viewModel, String recipent)
        {
            //create notification 
            Notification notification = viewModel;
            notification.GeneratedAt = DateTime.Now;
            notification.IsRead = false;
            notification.Recipient = recipent;
            notification.RecipientNavigation = notification.RecipientNavigation;

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                _context.Add(notification);
                await _context.SaveChangesAsync();
                await NotificationBroadcast();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Recipient"] = new SelectList(_context.Users, "Username", "Username", notification.Recipient);
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            ViewData["Recipient"] = new SelectList(_context.Users, "Username", "Username", notification.Recipient);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NotificationId,NotificationText,Recipient,IsRead,GeneratedAt")] Notification notification)
        {
            if (id != notification.NotificationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                    await NotificationBroadcast();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.NotificationId))
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
            ViewData["Recipient"] = new SelectList(_context.Users, "Username", "Username", notification.Recipient);
            return View(notification);
        }

        // GET: Notifications/Delete/5

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.RecipientNavigation)
                .FirstOrDefaultAsync(m => m.NotificationId == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        [HttpDelete]
        [Route("Notifications/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Notifications == null)
            {
                return Problem("Entity set 'ProjectManagementContext.Notifications'  is null.");
            }
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }

            await _context.SaveChangesAsync();
            await NotificationBroadcast();
            return Redirect("/Notifications/Client");
        }


        [HttpPut]
        [Route("Notifications/MarkAsRead/{id}")]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {

            // If the notification doesn't exist return 
            if (_context.Notifications == null)
            {
                return Problem("Entity set 'ProjectManagementContext.Notifications'  is null.");
            }

            // Find the notification with the specified ID in the database
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true; // Mark the notification as read
                _context.Update(notification);
                // and save changes to the database
                _context.SaveChanges();
                await NotificationBroadcast();
            }

            // Return to the notifications page
            return Redirect("/Notifications/Client");

        }



        private bool NotificationExists(int id)
        {
            return (_context.Notifications?.Any(e => e.NotificationId == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> Client()
        {
            return View();
        }
        ////for a specific user 
        [Route("Notifications/GetForUser")]
        public IActionResult GetForUser(string username)
        {
            var notifications = _context.Notifications
                .Where(n => n.Recipient == username)
                .OrderByDescending(n => n.GeneratedAt)
                .ToList();

            return Json(notifications);
        }
        public async Task<JsonResult> GetAll()
        {
            return Json(_context.Notifications.ToList());
        }
        public async System.Threading.Tasks.Task NotificationBroadcast()
        {
            await _hubcontext.NotificationsHubBroadcast(_context.Notifications.ToList());
        }
        //public static async System.Threading.Tasks.Task PushNotification(String recipient, String message, ProjectManagementContext context)
        //{
        //    // create notification obj
        //    var notification = new Notification
        //    {
        //        GeneratedAt = DateTime.Now,
        //        IsRead = false,
        //        Recipient = recipient,
        //        NotificationText = message,

        //    };
        //    context.Notifications.Add(notification);
        //    await context.SaveChangesAsync();

        //}

        public static async System.Threading.Tasks.Task<List<Notification>> PushNotification(string recipient, string message, ProjectManagementContext context, IHubContext<NotificationsHub> hubContext)
        {
            // Create a new notification object
            var notification = new Notification
            {
                GeneratedAt = DateTime.Now,
                IsRead = false,
                Recipient = recipient,
                NotificationText = message
            };

            // Add the notification to the database
            context.Notifications.Add(notification);
            await context.SaveChangesAsync();

            // Get all notifications from the database, including the new notification
            var notifications = await context.Notifications.ToListAsync();

            // Broadcast the updated list of notifications to all clients
            await hubContext.Clients.All.SendAsync("getUpdatedNotifications", notifications);
           return notifications;
        }
    }
}
