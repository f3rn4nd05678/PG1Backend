using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Extensions;
using ProyectoGraduación.Services;
using ProyectoGraduación.Services.IServices;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProveedorController : ControllerBase
{
    private readonly IProveedorService _proveedorService;
    private readonly ILogger<ProveedorController> _logger;

    public ProveedorController(IProveedorService proveedorService, ILogger<ProveedorController> logger)
    {
        _proveedorService = proveedorService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todos los proveedores activos
    /// </summary>
    [HttpGet("listar")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var proveedores = await _proveedorService.GetAll();
            return this.ApiOk(proveedores, "Proveedores obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los proveedores");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener proveedores con filtros y paginación
    /// </summary>
    [HttpPost("listar")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> GetWithFilters([FromBody] FiltroProveedorDto? filtro = null)
    {
        try
        {
            filtro ??= new FiltroProveedorDto();

            var resultado = await _proveedorService.GetWithFilters(filtro);
            var proveedores = resultado.proveedores.ToList();
            var total = resultado.total;

            var response = new
            {
                Proveedores = proveedores,
                Total = total,
                Pagina = filtro.NumeroPagina,
                TamanoPagina = filtro.TamanoPagina,
                TotalPaginas = (int)Math.Ceiling(total / (double)filtro.TamanoPagina)
            };

            return this.ApiOk(response, "Proveedores obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proveedores con filtros");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener proveedor por ID
    /// </summary>
    [HttpPost("obtener")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> GetById([FromBody] IdRequest request)
    {
        try
        {
            var proveedor = await _proveedorService.GetById(request.Id);
            if (proveedor == null)
            {
                return this.ApiNotFound($"No se encontró el proveedor con ID {request.Id}");
            }

            return this.ApiOk(proveedor, "Proveedor obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proveedor por ID {Id}", request.Id);
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Buscar proveedores por nombre
    /// </summary>
    [HttpPost("buscar")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> Search([FromBody] SearchRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Termino))
            {
                return this.ApiError("El término de búsqueda es requerido");
            }

            var proveedores = await _proveedorService.SearchByNombre(request.Termino);
            return this.ApiOk(proveedores, "Búsqueda completada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar proveedores");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Crear nuevo proveedor
    /// </summary>
    [HttpPost("crear")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> Create([FromBody] CrearProveedorDto crearProveedorDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return this.ApiError("Datos inválidos", 400);
            }

            var proveedor = await _proveedorService.CreateProveedor(crearProveedorDto);
            return this.ApiCreated(proveedor, "Proveedor creado exitosamente");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al crear proveedor");
            return this.ApiError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear proveedor");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Actualizar proveedor existente
    /// </summary>
    [HttpPost("actualizar")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> Update([FromBody] UpdateProveedorRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return this.ApiError("Datos inválidos", 400);
            }

            var proveedor = await _proveedorService.UpdateProveedor(request.Id, request.Datos);
            return this.ApiOk(proveedor, "Proveedor actualizado exitosamente");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Proveedor no encontrado");
            return this.ApiNotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al actualizar proveedor");
            return this.ApiError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar proveedor");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Eliminar proveedor (soft delete)
    /// </summary>
    [HttpPost("eliminar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete([FromBody] IdRequest request)
    {
        try
        {
            await _proveedorService.DeleteProveedor(request.Id);
            return this.ApiOk("Proveedor eliminado exitosamente");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Proveedor no encontrado");
            return this.ApiNotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "No se puede eliminar el proveedor");
            return this.ApiError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar proveedor");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Validar si un nombre ya existe
    /// </summary>
    [HttpPost("validar-nombre")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> ValidarNombre([FromBody] ValidarNombreRequest request)
    {
        try
        {
            var existe = await _proveedorService.ExisteNombre(request.Nombre, request.ExcluirId);
            return this.ApiOk(new { Existe = existe }, existe ? "El nombre ya existe" : "El nombre está disponible");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar nombre");
            return this.ApiError(ex.Message);
        }
    }
}

// Clases auxiliares para requests
public class UpdateProveedorRequest
{
    public int Id { get; set; }
    public ActualizarProveedorDto Datos { get; set; } = null!;
}

public class ValidarNombreRequest
{
    public string Nombre { get; set; } = string.Empty;
    public int? ExcluirId { get; set; }
}