using Application.Commond.Interface;
using Microsoft.AspNetCore.SignalR;
using WebUI.Hubs;

namespace WebUI.SignalR
{
    public class SignalRNotificationService(IHubContext<NotificationsHub> hubContext) : INotificationService
    {
        public Task NotifyAllAsync(string eventName, object payload, CancellationToken cancellationToken = default)
        {
            return hubContext.Clients.All.SendAsync(eventName, payload, cancellationToken);
        }

        public Task NotifyGroupAsync(string groupName, string eventName, object payload, CancellationToken cancellationToken = default)
        {
            return hubContext.Clients.Group(groupName).SendAsync(eventName, payload, cancellationToken);
        }
    }
}
