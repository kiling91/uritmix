using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Service.Notification.SendToken;

public class SendToken : ISendToken
{
    private readonly ILogger<SendToken> _logger;
    private readonly IOptions<NotificationOptions> _options;

    public SendToken(ILogger<SendToken> logger, IOptions<NotificationOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task SendActivatePersonToken(string token)
    {
        var opt = _options.Value;
        var url = $"{opt.ActivatePersonUrl}/{token}";
        _logger.LogInformation("Activate person url: {Url}", url);
        await Task.CompletedTask;
    }

    public async Task SendResetPasswordToken(string token)
    {
        var opt = _options.Value;
        var url = $"{opt.ResetPasswordUrl}/{token}";
        _logger.LogInformation("Reset password url: {Url}", url);
        await Task.CompletedTask;
    }
}