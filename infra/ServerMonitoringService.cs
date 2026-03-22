using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerHost.Domain.Enums;
using ServerHost.Domain.Interfaces;

namespace ServerHost.Infrastructure;

public class ServerMonitoringService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ServerMonitoringService> _logger;

    public ServerMonitoringService(IServiceProvider serviceProvider, ILogger<ServerMonitoringService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Server Monitoring Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncServerStatuses(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while syncing server statuses.");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        _logger.LogInformation("Server Monitoring Service is stopping.");
    }

    private async Task SyncServerStatuses(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IGameServerRepository>();
        var dockerService = scope.ServiceProvider.GetRequiredService<IDockerService>();

        var servers = await repository.GetAllAsync();

        foreach (var server in servers)
        {
            if (stoppingToken.IsCancellationRequested) break;

            bool isActuallyRunning = await dockerService.IsRunningAsync(server.Name);
            var expectedStatus = isActuallyRunning ? ServerStatus.Running : ServerStatus.Stopped;

            if (server.Status != expectedStatus)
            {
                _logger.LogInformation("Updating server {ServerName} status from {OldStatus} to {NewStatus}", 
                    server.Name, server.Status, expectedStatus);
                
                if (expectedStatus == ServerStatus.Running)
                    server.Start(); // This handles logic and LastAccessedAt
                else
                    server.Stop();

                await repository.UpdateAsync(server);
            }
        }
    }
}
