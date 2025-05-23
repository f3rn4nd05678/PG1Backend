using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.IServices;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.Extensions;
using System.Security.Claims;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;
    private readonly IUsuarioRepository _usuarioRepository;

    public MenuController(IMenuService menuService, IUsuarioRepository usuarioRepository)
    {
        _menuService = menuService;
        _usuarioRepository = usuarioRepository;
    }

    [HttpGet("Obtener-menu")]
    public async Task<IActionResult> ObtenerMenu()
    {
        try
        {
 
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return this.ApiError("Token inválido");

     
            var usuario = await _usuarioRepository.GetById(int.Parse(userId));
            if (usuario == null)
                return this.ApiNotFound("Usuario no encontrado");

        
            var menus = await _menuService.GetMenusByRolId(usuario.RolId);

            return this.ApiOk(menus, "Menús obtenidos correctamente");
        }
        catch (Exception ex)
        {
            return this.ApiError(ex.Message);
        }
    }

    [HttpGet("Obtener-todos-menus")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerTodosLosMenus()
    {
        try
        {
            var menus = await _menuService.GetAll();
            return this.ApiOk(menus, "Todos los menús obtenidos correctamente");
        }
        catch (Exception ex)
        {
            return this.ApiError(ex.Message);
        }
    }
}