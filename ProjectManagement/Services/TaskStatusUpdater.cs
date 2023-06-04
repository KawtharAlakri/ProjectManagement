using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Controllers;
using ProjectManagement.Hubs;
using ProjectManagement.Models;
using System;

namespace ProjectManagement.Services
{
    public class TaskStatusUpdater
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<NotificationsHub> _hubContext;


        public TaskStatusUpdater(IServiceProvider serviceProvider, IHubContext<NotificationsHub> hubContext)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        public async System.Threading.Tasks.Task UpdateTaskStatusAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ProjectManagementContext>();

                // Update incompleted tasks status to be overdue once the date has passed
                var tasks = dbContext.Tasks.Where(t => t.Status == "in progress" && t.DueDate < DateTime.Now);
                foreach (var task in tasks)
                {
                    task.Status = "overdue";
                    // Save the changes to the task
                    dbContext.Tasks.Update(task);
                }
                // Save all changes to the database
                dbContext.SaveChanges();


                // Update incompleted projects status to be overdue once the date has passed
                var projects = dbContext.Projects.Where(t => t.Status == "in progress" && t.DueDate < DateTime.Now);
                foreach (var project in projects)
                {
                    project.Status = "overdue";
                    // Save the changes to the task
                    dbContext.Projects.Update(project);
                }
                // Save all changes to the database
                dbContext.SaveChanges();
            }
        }

    }
}
