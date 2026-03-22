using ServerHost.Application.DTOs;
using ServerHost.Domain.Entities;
using ServerHost.Domain.Enums;
using ServerHost.Domain.Interfaces;

namespace ServerHost.Application.Services;

public class GameServerService : IGameServerService
{
    private readonly IGameServerRepository _repository;
    private readonly IDockerService _dockerService;

    public GameServerService(IGameServerRepository repository, IDockerService dockerService)
    {
        _repository = repository;
        _dockerService = dockerService;
    }

    public async Task<IEnumerable<GameServerDto>> GetAllAsync()
    {
        var servers = await _repository.GetAllAsync();
        return servers.Select(s => new GameServerDto(s.Id, s.Name, s.Port, s.Status, s.CreatedAt, s.LastAccessedAt));
    }

    public async Task<GameServerDto?> GetByIdAsync(Guid id)
    {
        var s = await _repository.GetByIdAsync(id);
        return s == null ? null : new GameServerDto(s.Id, s.Name, s.Port, s.Status, s.CreatedAt, s.LastAccessedAt);
    }

    public async Task<GameServerDto> CreateAsync(CreateServerDto dto)
    {
        if (await _repository.ExistsByNameAsync(dto.Name))
            throw new InvalidOperationException("Server name already exists.");

        int maxPort = await _repository.GetMaxPortAsync();
        int nextPort = maxPort + 1;

        if (nextPort < 25565) nextPort = 25565;

        var server = new GameServer
        {
            Name = dto.Name,
            Port = nextPort,
            Status = ServerStatus.Stopped
        };

        await _repository.AddAsync(server);
        await _dockerService.CreateContainerAsync(server.Name, server.Port);

        return new GameServerDto(server.Id, server.Name, server.Port, server.Status, server.CreatedAt, server.LastAccessedAt);
    }

    public async Task StartAsync(Guid id)
    {
        var server = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        
        await _dockerService.StartContainerAsync(server.Name);
        server.Start();
        await _repository.UpdateAsync(server);
    }

    public async Task StopAsync(Guid id)
    {
        var server = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        
        await _dockerService.StopContainerAsync(server.Name);
        server.Stop();
        await _repository.UpdateAsync(server);
    }

    public async Task DeleteAsync(Guid id)
    {
        var server = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        
        await _dockerService.RemoveContainerAsync(server.Name);
        await _repository.DeleteAsync(id);
    }
}
