using Application.Commond.Interface;
using Application.Dtos;

namespace WebUI.SignalR
{
    public class NotificationsFacade(INotificationService notificationService) : INotificationsFacade
    {
        private const string NotificationReceivedEvent = "notification-received";

        public Task BroadcastAsync(string title, string message, string type = "info", CancellationToken cancellationToken = default)
        {
            var payload = new NotificationMessageDto
            {
                Title = title,
                Message = message,
                Type = type
            };

            return notificationService.NotifyAllAsync(NotificationReceivedEvent, payload, cancellationToken);
        }
    }
}
