using Microsoft.EntityFrameworkCore;
using ServerHost.Domain.Entities;
using ServerHost.Domain.Interfaces;

namespace ServerHost.Infrastructure.Persistance;

public class GameServerRepository : IGameServerRepository
{
    private readonly AppDbContext _context;

    public GameServerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GameServer?> GetByIdAsync(Guid id)
    {
        return await _context.Servers.FindAsync(id);
    }

    public async Task<IEnumerable<GameServer>> GetAllAsync()
    {
        return await _context.Servers.ToListAsync();
    }

    public async Task AddAsync(GameServer server)
    {
        await _context.Servers.AddAsync(server);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(GameServer server)
    {
        _context.Servers.Update(server);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var server = await _context.Servers.FindAsync(id);
        if (server != null)
        {
            _context.Servers.Remove(server);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Servers.AnyAsync(s => s.Name == name);
    }

    public async Task<bool> ExistsByPortAsync(int port)
    {
        return await _context.Servers.AnyAsync(s => s.Port == port);
    }

    public async Task<int> GetMaxPortAsync()
    {
        if (!await _context.Servers.AnyAsync())
            return 25564; // Base port - 1

        return await _context.Servers.MaxAsync(s => s.Port);
    }
}
