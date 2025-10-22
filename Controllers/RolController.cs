using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Models;
using ProyectoGraduación.Extensions;
using System.Linq;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class RolController : ControllerBase
{
    private readonly IRolService _rolService;
    private readonly ILogger<RolController> _logger;

    public RolController(IRolService rolService, ILogger<RolController> logger)
    {
        _rolService = rolService;
        _logger = logger;
    }

    /// <summary>
    /// Listar todos los roles
    /// </summary>
    [HttpPost("listar")]
    public async Task<IActionResult> Listar([FromBody] FiltroRolDto? filtro = null)
    {
        try
        {
            var roles = await _rolService.GetAll();

            // Aplicar filtro de búsqueda si existe
            if (filtro != null && !string.IsNullOrWhiteSpace(filtro.TerminoBusqueda))
            {
                var termino = filtro.TerminoBusqueda.ToLower();
                roles = roles.Where(r =>
                    r.Nombre.ToLower().Contains(termino) ||
                    (r.Descripcion != null && r.Descripcion.ToLower().Contains(termino))
                );
            }

            var rolesDto = roles.Select(r => new RolDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion
            }).ToList();

            return this.ApiOk(rolesDto, "Roles obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar roles");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener rol por ID
    /// </summary>
    [HttpPost("obtener")]
    public async Task<IActionResult> Obtener([FromBody] ObtenerRolPorIdDto request)
    {
        try
        {
            var rol = await _rolService.GetById(request.Id);
            if (rol == null)
                return this.ApiNotFound("Rol no encontrado");

            var rolDto = new RolDto
            {
                Id = rol.Id,
                Nombre = rol.Nombre,
                Descripcion = rol.Descripcion
            };

            return this.ApiOk(rolDto, "Rol obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener rol");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Crear nuevo rol
    /// </summary>
    [HttpPost("crear")]
    public async Task<IActionResult> Crear([FromBody] CrearRolDto rolDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return this.ApiError("Datos inválidos", 400);

            var rol = new Rol
            {
                Nombre = rolDto.Nombre,
                Descripcion = rolDto.Descripcion
            };

            await _rolService.Add(rol);

            var resultado = new RolDto
            {
                Id = rol.Id,
                Nombre = rol.Nombre,
                Descripcion = rol.Descripcion
            };

            return this.ApiCreated(resultado, "Rol creado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear rol");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Actualizar rol existente
    /// </summary>
    [HttpPost("actualizar")]
    public async Task<IActionResult> Actualizar([FromBody] ActualizarRolRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return this.ApiError("Datos inválidos", 400);

            var rol = await _rolService.GetById(request.Id);
            if (rol == null)
                return this.ApiNotFound("Rol no encontrado");

            // Proteger roles del sistema
            var rolesSistema = new[] { "Administrador", "Punto de Venta", "Bodega", "Vendedor" };
            if (rolesSistema.Contains(rol.Nombre))
                return this.ApiError("No se pueden modificar los roles predefinidos del sistema");

            rol.Nombre = request.Datos.Nombre;
            rol.Descripcion = request.Datos.Descripcion;

            await _rolService.Update(rol);

            var resultado = new RolDto
            {
                Id = rol.Id,
                Nombre = rol.Nombre,
                Descripcion = rol.Descripcion
            };

            return this.ApiOk(resultado, "Rol actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar rol");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Eliminar rol
    /// </summary>
    [HttpPost("eliminar")]
    public async Task<IActionResult> Eliminar([FromBody] EliminarRolDto request)
    {
        try
        {
            var rol = await _rolService.GetById(request.Id);
            if (rol == null)
                return this.ApiNotFound("Rol no encontrado");

            // Proteger roles del sistema
            var rolesSistema = new[] { "Administrador", "Punto de Venta", "Bodega", "Vendedor" };
            if (rolesSistema.Contains(rol.Nombre))
                return this.ApiError("No se pueden eliminar los roles predefinidos del sistema");

            await _rolService.Delete(request.Id);
            return this.ApiOk(null, "Rol eliminado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar rol");
            return this.ApiError(ex.Message);
        }
    }
}