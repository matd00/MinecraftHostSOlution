using ServerHost.Application.DTOs;

namespace ServerHost.Application.Services;

public interface IGameServerService
{
    Task<IEnumerable<GameServerDto>> GetAllAsync();
    Task<GameServerDto?> GetByIdAsync(Guid id);
    Task<GameServerDto> CreateAsync(CreateServerDto dto);
    Task StartAsync(Guid id);
    Task StopAsync(Guid id);
    Task DeleteAsync(Guid id);
}
