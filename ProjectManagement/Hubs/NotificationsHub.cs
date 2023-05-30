using Microsoft.AspNetCore.SignalR;
using ProjectManagement.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ProjectManagement.Hubs
{
    public class NotificationsHub : Hub
    {
        IHubContext<NotificationsHub> _hubContext;
        public NotificationsHub(IHubContext<NotificationsHub> hubcontext)
        {
            _hubContext = hubcontext;
        }
        public async System.Threading.Tasks.Task NotificationsHubBroadcast(List<Notification> notifications)
        {
            await _hubContext.Clients.All.SendAsync("getUpdatedNotifications", notifications);

        }
    }
}

