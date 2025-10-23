using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Extensions;
using System.Security.Claims;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BodegaController : ControllerBase
{
    private readonly IBodegaService _bodegaService;
    private readonly ILogger<BodegaController> _logger;

    public BodegaController(IBodegaService bodegaService, ILogger<BodegaController> logger)
    {
        _bodegaService = bodegaService;
        _logger = logger;
    }

    private int ObtenerUsuarioId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    /// <summary>
    /// Obtener todas las bodegas activas (sin paginación)
    /// </summary>
    [HttpGet("listar")]
    [Authorize(Roles = "Administrador,Bodega,Vendedor")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var bodegas = await _bodegaService.GetAll();
            return this.ApiOk(bodegas, "Bodegas obtenidas correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las bodegas");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener bodegas con filtros y paginación
    /// </summary>
    [HttpPost("listar")]
    [Authorize(Roles = "Administrador,Bodega,Vendedor")]
    public async Task<IActionResult> GetWithFilters([FromBody] FiltroBodegaDto? filtro = null)
    {
        try
        {
            filtro ??= new FiltroBodegaDto();

            if (filtro.ElementosPorPagina <= 0)
                filtro.ElementosPorPagina = 10;

            var (bodegas, total) = await _bodegaService.GetWithFilters(filtro);

            var response = new ListadoBodegasResponseDto
            {
                Bodegas = bodegas.ToList(),
                Total = total,
                Pagina = filtro.Pagina,
                ElementosPorPagina = filtro.ElementosPorPagina,
                TotalPaginas = (int)Math.Ceiling((double)total / filtro.ElementosPorPagina)
            };

            return this.ApiOk(response, "Bodegas obtenidas correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener bodegas con filtros");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener bodega por ID
    /// </summary>
    [HttpPost("obtener")]
    [Authorize(Roles = "Administrador,Bodega,Vendedor")]
    public async Task<IActionResult> GetById([FromBody] ObtenerBodegaPorIdDto request)
    {
        try
        {
            var bodega = await _bodegaService.GetById(request.Id);
            if (bodega == null)
                return this.ApiNotFound("Bodega no encontrada");

            return this.ApiOk(bodega, "Bodega obtenida correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener bodega por ID");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Crear nueva bodega
    /// </summary>
    [HttpPost("crear")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> Create([FromBody] CrearBodegaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return this.ApiError("Datos inválidos", 400);

            var usuarioId = ObtenerUsuarioId();
            var bodega = await _bodegaService.Create(dto, usuarioId);

            return this.ApiCreated(bodega, "Bodega creada correctamente");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al crear bodega");
            return this.ApiError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear bodega");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Actualizar bodega existente
    /// </summary>
    [HttpPost("actualizar")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> Update([FromBody] UpdateBodegaRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return this.ApiError("Datos inválidos", 400);

            var usuarioId = ObtenerUsuarioId();
            var bodega = await _bodegaService.Update(request.Id, request.Datos, usuarioId);

            return this.ApiOk(bodega, "Bodega actualizada correctamente");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Bodega no encontrada");
            return this.ApiNotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al actualizar bodega");
            return this.ApiError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar bodega");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Eliminar bodega (soft delete)
    /// </summary>
    [HttpPost("eliminar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete([FromBody] ObtenerBodegaPorIdDto request)
    {
        try
        {
            var usuarioId = ObtenerUsuarioId();
            var resultado = await _bodegaService.Delete(request.Id, usuarioId);

            if (!resultado)
                return this.ApiNotFound("Bodega no encontrada");

            return this.ApiOk("Bodega eliminada correctamente");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "No se puede eliminar la bodega");
            return this.ApiError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar bodega");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Cambiar estado de bodega (activar/desactivar)
    /// </summary>
    [HttpPost("cambiar-estado")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> CambiarEstado([FromBody] CambiarEstadoBodegaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return this.ApiError("Datos inválidos", 400);

            var usuarioId = ObtenerUsuarioId();
            var resultado = await _bodegaService.CambiarEstado(dto.Id, dto.Activa, usuarioId);

            if (!resultado)
                return this.ApiNotFound("Bodega no encontrada");

            var mensaje = dto.Activa ? "Bodega activada correctamente" : "Bodega desactivada correctamente";
            return this.ApiOk(mensaje);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado de bodega");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener alertas de stock de una bodega
    /// </summary>
    [HttpGet("{id}/alertas")]
    [Authorize(Roles = "Administrador,Bodega,Vendedor")]
    public async Task<IActionResult> GetAlertasStock(int id)
    {
        try
        {
            var alertas = await _bodegaService.GetAlertasStock(id);
            return this.ApiOk(alertas, "Alertas de stock obtenidas correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener alertas de stock");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Validar si existe una bodega con el nombre especificado
    /// </summary>
    [HttpPost("validar-nombre")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> ValidarNombre([FromBody] ValidarNombreBodegaRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                return this.ApiError("El nombre es requerido");

            var existe = await _bodegaService.ExisteNombre(request.Nombre, request.ExcluirId);
            return this.ApiOk(
                new { Existe = existe },
                existe ? "El nombre ya existe" : "El nombre está disponible"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar nombre de bodega");
            return this.ApiError(ex.Message);
        }
    }
}

// Clases auxiliares para requests
public class UpdateBodegaRequest
{
    public int Id { get; set; }
    public ActualizarBodegaDto Datos { get; set; } = null!;
}

public class ValidarNombreBodegaRequest
{
    public string Nombre { get; set; } = string.Empty;
    public int? ExcluirId { get; set; }
}