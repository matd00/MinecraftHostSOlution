using Microsoft.AspNetCore.Mvc;
using ServerHost.Application.DTOs;
using ServerHost.Application.Services;

namespace ServerHost.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServersController : ControllerBase
{
    private readonly IGameServerService _service;

    public ServersController(IGameServerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameServerDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameServerDto>> GetById(Guid id)
    {
        var server = await _service.GetByIdAsync(id);
        if (server == null) return NotFound();
        return Ok(server);
    }

    [HttpPost]
    public async Task<ActionResult<GameServerDto>> Create(CreateServerDto dto)
    {
        try
        {
            var server = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = server.Id }, server);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> Start(Guid id)
    {
        try
        {
            await _service.StartAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/stop")]
    public async Task<IActionResult> Stop(Guid id)
    {
        try
        {
            await _service.StopAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
