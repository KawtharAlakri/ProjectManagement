using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManagement.Services
{
    public class TaskStatusUpdateService : BackgroundService
    {
        private readonly TaskStatusUpdater _taskStatusUpdater;

        public TaskStatusUpdateService(TaskStatusUpdater taskStatusUpdater)
        {
            _taskStatusUpdater = taskStatusUpdater;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Perform the task status update
                _taskStatusUpdater.UpdateTaskStatusAsync();

                // Wait for 1 hour before the next update
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}