using Microsoft.AspNetCore.SignalR;
using ProjectManagement.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagement.Hubs
{
    public class NotificationsHub : Hub
    {
        public async System.Threading.Tasks.Task NotificationsHubBroadcast(List<Notification> notifications)
        {
            await Clients.All.SendAsync("getUpdatedNotifications", notifications);
            // Send the notification to the client
            
        }
    }
}

