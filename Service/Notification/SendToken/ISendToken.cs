namespace Service.Notification.SendToken;

public interface ISendToken
{
    Task SendActivatePersonToken(string token);
    Task SendResetPasswordToken(string token);
}