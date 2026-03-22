using ServerHost.Domain.Entities;

namespace ServerHost.Domain.Interfaces;

public interface IGameServerRepository
{
    Task<GameServer?> GetByIdAsync(Guid id);
    Task<IEnumerable<GameServer>> GetAllAsync();
    Task AddAsync(GameServer server);
    Task UpdateAsync(GameServer server);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name);
    Task<bool> ExistsByPortAsync(int port);
    Task<int> GetMaxPortAsync();
}
