using Application.Commond.Interface;
using Application.Dtos;

namespace WebUI.SignalR
{
    public class NotificationsFacade(INotificationService notificationService) : INotificationsFacade
    {
        private const string NotificationReceivedEvent = "notification-received";

        public Task BroadcastAsync(Guid tenantId, string title, string message, string type = "info", CancellationToken cancellationToken = default, object? data = null)
        {
            var payload = new NotificationMessageDto
            {
                Title = title,
                Message = message,
                Type = type,
                Data = data
            };

            var groupName = Hubs.NotificationsHub.GetTenantGroupName(tenantId);
            return notificationService.NotifyGroupAsync(groupName, NotificationReceivedEvent, payload, cancellationToken);
        }
    }
}
