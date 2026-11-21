namespace FreshCart.Common.Notifications;

public sealed class UserNotification
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime? ReadAt { get; set; }

    public DateTime CreatedOn { get; set; }
}
