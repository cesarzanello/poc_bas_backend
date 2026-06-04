namespace Application.Commond.Interface
{
    public interface INotificationService
    {
        Task NotifyAllAsync(string eventName, object payload, CancellationToken cancellationToken = default);
    }

    public interface INotificationsFacade
    {
        Task BroadcastAsync(string title, string message, string type = "info", CancellationToken cancellationToken = default, object? data = null);
    }
}
