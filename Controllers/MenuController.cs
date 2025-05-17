using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.Models;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Services.IServices;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MenuController : ControllerBase
{
    private readonly IMenuService _service;

    public MenuController(IMenuService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAll());

    [HttpGet("por-rol/{rolId}")]
    public async Task<IActionResult> GetMenusByRolId(int rolId)
    {
        return Ok(await _service.GetMenusByRolId(rolId));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> GetById(int id)
    {
        var menu = await _service.GetById(id);
        if (menu == null)
            return NotFound();
        return Ok(menu);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Post([FromBody] Menu menu)
    {
        await _service.Add(menu);
        return CreatedAtAction(nameof(GetById), new { id = menu.Id }, menu);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Put(int id, [FromBody] Menu menu)
    {
        menu.Id = id;
        await _service.Update(menu);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}