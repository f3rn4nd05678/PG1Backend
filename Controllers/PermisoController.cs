using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Extensions;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class PermisoController : ControllerBase
{
    private readonly IPermisoService _permisoService;
    private readonly ILogger<PermisoController> _logger;

    public PermisoController(IPermisoService permisoService, ILogger<PermisoController> logger)
    {
        _permisoService = permisoService;
        _logger = logger;
    }

    /// <summary>
    /// Listar todos los permisos
    /// </summary>
    [HttpPost("listar")]
    public async Task<IActionResult> Listar()
    {
        try
        {
            var permisos = await _permisoService.GetAll();

            var permisosDto = permisos.Select(p => new PermisoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion
            }).ToList();

            return this.ApiOk(permisosDto, "Permisos obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar permisos");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener permiso por ID
    /// </summary>
    [HttpPost("obtener")]
    public async Task<IActionResult> Obtener([FromBody] int id)
    {
        try
        {
            var permiso = await _permisoService.GetById(id);
            if (permiso == null)
                return this.ApiNotFound("Permiso no encontrado");

            var permisoDto = new PermisoDto
            {
                Id = permiso.Id,
                Nombre = permiso.Nombre,
                Descripcion = permiso.Descripcion
            };

            return this.ApiOk(permisoDto, "Permiso obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener permiso");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Listar permisos de un rol específico
    /// </summary>
    [HttpPost("listar-por-rol")]
    public async Task<IActionResult> ListarPorRol([FromBody] ListarPermisosPorRolDto request)
    {
        try
        {
            var permisos = await _permisoService.GetPermisosByRolId(request.RolId);

            var permisosDto = permisos.Select(p => new PermisoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion
            }).ToList();

            return this.ApiOk(permisosDto, "Permisos del rol obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar permisos por rol");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Asignar permiso a un rol
    /// </summary>
    [HttpPost("asignar-rol")]
    public async Task<IActionResult> AsignarPermisoARol([FromBody] AsignarPermisoRequest request)
    {
        try
        {
            await _permisoService.AsignarPermisoARol(request.RolId, request.PermisoId);
            return this.ApiOk(null, "Permiso asignado al rol exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar permiso a rol");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Remover permiso de un rol
    /// </summary>
    [HttpPost("remover-rol")]
    public async Task<IActionResult> RemoverPermisoDeRol([FromBody] RemoverPermisoRequest request)
    {
        try
        {
            await _permisoService.RemoverPermisoDeRol(request.RolId, request.PermisoId);
            return this.ApiOk(null, "Permiso removido del rol exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al remover permiso de rol");
            return this.ApiError(ex.Message);
        }
    }
}