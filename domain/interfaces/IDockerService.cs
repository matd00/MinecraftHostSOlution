namespace ServerHost.Domain.Interfaces;

public interface IDockerService
{
    Task CreateContainerAsync(string name, int port);
    Task StartContainerAsync(string name);
    Task StopContainerAsync(string name);
    Task RemoveContainerAsync(string name);
    Task<bool> IsRunningAsync(string name);
}
