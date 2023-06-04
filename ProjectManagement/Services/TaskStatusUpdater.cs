using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Models;
using System;
namespace ProjectManagement.Services
{
    public class TaskStatusUpdater
    {
        private readonly IServiceProvider _serviceProvider;

        public TaskStatusUpdater(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void UpdateTaskStatus()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ProjectManagementContext>();
                

                //update incompleted tasks status to beoverde once date has passed
                var tasks = dbContext.Tasks.ToList();

                foreach (var task in tasks)
                {
                    if (task.DueDate < DateTime.Today && task.Status == "in progress")
                    {
                        task.Status = "overdue";
                    }
                }

                //update incompleted projects status to beoverde once date has passed
                var projects = dbContext.Projects.ToList(); 
                foreach (var project in projects) 
                { 
                    if (project.DueDate < DateTime.Today && project.Status == "in progress")
                    {
                        project.Status = "overdue";
                    }
                }
                dbContext.SaveChanges();
            }
        }
    }
}
