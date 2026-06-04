namespace Application.Dtos
{
    public class NotificationMessageDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "info";
        public DateTime SentAtUtc { get; set; } = DateTime.UtcNow;
    }
}
