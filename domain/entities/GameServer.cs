using ServerHost.Domain.Enums;

namespace ServerHost.Domain.Entities;

public class GameServer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int Port { get; set; }
    public ServerStatus Status { get; set; } = ServerStatus.Stopped;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastAccessedAt { get; set; }

    public void Start()
    {
        if (Status == ServerStatus.Running)
            throw new InvalidOperationException("Server is already running.");
        
        Status = ServerStatus.Running;
        LastAccessedAt = DateTime.UtcNow;
    }

    public void Stop()
    {
        if (Status == ServerStatus.Stopped)
            throw new InvalidOperationException("Server is already stopped.");
        
        Status = ServerStatus.Stopped;
    }
}
