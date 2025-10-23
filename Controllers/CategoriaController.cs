using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.Extensions;
using ProyectoGraduación.Services.IServices;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;
    private readonly ILogger<CategoriaController> _logger;

    public CategoriaController(ICategoriaService categoriaService, ILogger<CategoriaController> logger)
    {
        _categoriaService = categoriaService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todas las categorías
    /// </summary>
    [HttpGet("listar")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var categorias = await _categoriaService.ObtenerTodosAsync();
            return this.ApiOk(categorias, "Categorías obtenidas correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las categorías");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener solo categorías activas
    /// </summary>
    [HttpGet("activas")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta,Bodega")]
    public async Task<IActionResult> GetActivas()
    {
        try
        {
            var categorias = await _categoriaService.ObtenerActivosAsync();
            return this.ApiOk(categorias, "Categorías activas obtenidas correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorías activas");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener categoría por ID
    /// </summary>
    [HttpPost("obtener")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> GetById([FromBody] IdRequest request)
    {
        try
        {
            var categoria = await _categoriaService.ObtenerPorIdAsync(request.Id);
            if (categoria == null)
                return this.ApiNotFound($"No se encontró la categoría con ID {request.Id}");

            return this.ApiOk(categoria, "Categoría obtenida correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categoría por ID {Id}", request.Id);
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Crear nueva categoría
    /// </summary>
    [HttpPost("crear")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> Create([FromBody] CrearCategoriaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return this.ApiError("Datos inválidos", 400);

            var categoria = await _categoriaService.CrearAsync(dto);
            return this.ApiCreated(categoria, "Categoría creada exitosamente");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al crear categoría");
            return this.ApiError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear categoría");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Actualizar categoría existente
    /// </summary>
    [HttpPost("actualizar")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> Update([FromBody] UpdateCategoriaRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return this.ApiError("Datos inválidos", 400);

            var categoria = await _categoriaService.ActualizarAsync(request.Id, request.Datos);
            return this.ApiOk(categoria, "Categoría actualizada exitosamente");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Categoría no encontrada");
            return this.ApiNotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al actualizar categoría");
            return this.ApiError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar categoría");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Eliminar categoría (soft delete)
    /// </summary>
    [HttpPost("eliminar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete([FromBody] IdRequest request)
    {
        try
        {
            await _categoriaService.EliminarAsync(request.Id);
            return this.ApiOk("Categoría eliminada exitosamente");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Categoría no encontrada");
            return this.ApiNotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar categoría");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Validar si un código prefijo ya existe
    /// </summary>
    [HttpPost("validar-codigo")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> ValidarCodigo([FromBody] ValidarCodigoPrefijoRequest request)
    {
        try
        {
            var existe = await _categoriaService.ExisteCodigoPrefijo(request.CodigoPrefijo, request.ExcluirId);
            return this.ApiOk(new { Existe = existe }, existe ? "El código ya existe" : "El código está disponible");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar código prefijo");
            return this.ApiError(ex.Message);
        }
    }
}

// Clases auxiliares
public class UpdateCategoriaRequest
{
    public int Id { get; set; }
    public ActualizarCategoriaDto Datos { get; set; } = null!;
}

public class ValidarCodigoPrefijoRequest
{
    public string CodigoPrefijo { get; set; } = string.Empty;
    public int? ExcluirId { get; set; }
}