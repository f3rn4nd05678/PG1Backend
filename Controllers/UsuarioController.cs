using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IServices;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost("crear")]
    [Authorize(Roles = "Administrador")] 
    public async Task<IActionResult> CrearUsuario([FromBody] RegistroUsuarioDto usuarioDto)
    {
        try
        {
            var usuario = await _usuarioService.Registrar(usuarioDto);
            return Ok(new
            {
                message = "Usuario creado exitosamente",
                usuario = new
                {
                    id = usuario.Id,
                    nombre = usuario.Nombre,
                    correo = usuario.Correo,
                    rol = usuario.RolNombre
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}