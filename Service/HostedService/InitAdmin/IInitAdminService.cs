namespace Service.HostedService.InitAdmin;

public interface IInitAdminService
{
    Task DoWork(CancellationToken stoppingToken);
}