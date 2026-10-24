namespace WebApp;

public sealed class BlackboardConnectionService : BackgroundService
{
    public BlackboardConnectionService(ILogger<BlackboardConnectionService> logger) =>
        DWISConnector.Instance.ConnectionLogger = logger;

    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        Task.Run(() => DWISConnector.Instance.RunAsync(stoppingToken), stoppingToken);
}
