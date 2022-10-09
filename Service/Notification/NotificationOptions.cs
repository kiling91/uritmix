namespace Service.Notification;

public class NotificationOptions
{
    public const string Options = "NotificationOptions";
    public string SmtpHost { get; init; } = null!;
    public int SmtpPort { get; init; }
    public string SmtpUserName { get; init; } = null!;
    public string SmtpPassword { get; init; } = null!;
    public string DebugEmailConsumer { get; init; } = null!;
    public string ActivatePersonUrl { get; init; } = null!;
    public string ResetPasswordUrl { get; init; } = null!;
}