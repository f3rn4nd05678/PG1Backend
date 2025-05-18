using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.Models;
using ProyectoGraduación.IServices;
using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class PermisoController : ControllerBase
{
    private readonly IPermisoService _service;

    public PermisoController(IPermisoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAll());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var permiso = await _service.GetById(id);
        if (permiso == null)
            return NotFound();
        return Ok(permiso);
    }

    [HttpGet("por-rol/{rolId}")]
    public async Task<IActionResult> GetPermisosByRolId(int rolId)
    {
        return Ok(await _service.GetPermisosByRolId(rolId));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Permiso permiso)
    {
        await _service.Add(permiso);
        return CreatedAtAction(nameof(GetById), new { id = permiso.Id }, permiso);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Permiso permiso)
    {
        permiso.Id = id;
        await _service.Update(permiso);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }

    [HttpPost("asignar")]
    public async Task<IActionResult> AsignarPermisoARol([FromBody] AsignarPermisoDto dto)
    {
        await _service.AsignarPermisoARol(dto.RolId, dto.PermisoId);
        return Ok();
    }

    [HttpDelete("remover")]
    public async Task<IActionResult> RemoverPermisoDeRol([FromQuery] int rolId, [FromQuery] int permisoId)
    {
        await _service.RemoverPermisoDeRol(rolId, permisoId);
        return Ok();
    }
}