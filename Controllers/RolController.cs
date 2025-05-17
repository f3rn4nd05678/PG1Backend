using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.Models;
using ProyectoGraduación.IServices;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class RolController : ControllerBase
{
    private readonly IRolService _service;

    public RolController(IRolService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAll());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var rol = await _service.GetById(id);
        if (rol == null)
            return NotFound();
        return Ok(rol);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Rol rol)
    {
        await _service.Add(rol);
        return CreatedAtAction(nameof(GetById), new { id = rol.Id }, rol);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Rol rol)
    {
        rol.Id = id;
        await _service.Update(rol);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}