using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.Extensions;
using ProyectoGraduación.IServices;
using System.Security.Claims;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MovimientoInventarioController : ControllerBase
{
    private readonly IMovimientoInventarioService _movimientoService;
    private readonly ILogger<MovimientoInventarioController> _logger;

    public MovimientoInventarioController(
        IMovimientoInventarioService movimientoService,
        ILogger<MovimientoInventarioController> logger)
    {
        _movimientoService = movimientoService;
        _logger = logger;
    }

    private int ObtenerUsuarioId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    /// <summary>
    /// Obtener todos los movimientos de inventario
    /// </summary>
    [HttpGet("listar")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var movimientos = await _movimientoService.GetAll();
            return this.ApiOk(movimientos, "Movimientos obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimientos de inventario");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener movimientos con filtros y paginación
    /// </summary>
    [HttpPost("listar")]
    [Authorize(Roles = "Administrador,Bodega,Vendedor")]
    public async Task<IActionResult> GetWithFilters([FromBody] FiltroMovimientoInventarioDto? filtro = null)
    {
        try
        {
            filtro ??= new FiltroMovimientoInventarioDto();

            if (filtro.ElementosPorPagina <= 0)
                filtro.ElementosPorPagina = 10;

            var (movimientos, total) = await _movimientoService.GetWithFilters(filtro);

            var response = new ListadoMovimientosResponseDto
            {
                Movimientos = movimientos.ToList(),
                Total = total,
                Pagina = filtro.Pagina,
                ElementosPorPagina = filtro.ElementosPorPagina,
                TotalPaginas = (int)Math.Ceiling((double)total / filtro.ElementosPorPagina)
            };

            return this.ApiOk(response, "Movimientos obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimientos con filtros");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener movimiento por ID
    /// </summary>
    [HttpPost("obtener")]
    [Authorize(Roles = "Administrador,Bodega,Vendedor")]
    public async Task<IActionResult> GetById([FromBody] ObtenerMovimientoPorIdDto request)
    {
        try
        {
            var movimiento = await _movimientoService.GetById(request.Id);
            if (movimiento == null)
                return this.ApiNotFound("Movimiento no encontrado");

            return this.ApiOk(movimiento, "Movimiento obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimiento por ID");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Registrar entrada de inventario
    /// </summary>
    [HttpPost("entrada")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> RegistrarEntrada([FromBody] CrearEntradaInventarioDto entrada)
    {
        try
        {
            var usuarioId = ObtenerUsuarioId();
            if (usuarioId == 0)
                return this.ApiError("Usuario no autenticado");

            var movimiento = await _movimientoService.RegistrarEntrada(entrada, usuarioId);

            return this.ApiCreated(movimiento, "Entrada de inventario registrada correctamente");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al registrar entrada");
            return this.ApiError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar entrada de inventario");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Registrar salida de inventario
    /// </summary>
    [HttpPost("salida")]
    [Authorize(Roles = "Administrador,Bodega,Punto de venta")]
    public async Task<IActionResult> RegistrarSalida([FromBody] CrearEntradaInventarioDto salida)
    {
        try
        {
            var usuarioId = ObtenerUsuarioId();
            if (usuarioId == 0)
                return this.ApiError("Usuario no autenticado");

            var movimiento = await _movimientoService.RegistrarSalida(salida, usuarioId);

            return this.ApiCreated(movimiento, "Salida de inventario registrada correctamente");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al registrar salida");
            return this.ApiError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar salida de inventario");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener movimientos por producto
    /// </summary>
    [HttpGet("producto/{idProducto}")]
    [Authorize(Roles = "Administrador,Bodega,Vendedor")]
    public async Task<IActionResult> GetByProducto(int idProducto)
    {
        try
        {
            var movimientos = await _movimientoService.GetByProducto(idProducto);
            return this.ApiOk(movimientos, "Movimientos del producto obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimientos por producto");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener movimientos por bodega
    /// </summary>
    [HttpGet("bodega/{idBodega}")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> GetByBodega(int idBodega)
    {
        try
        {
            var movimientos = await _movimientoService.GetByBodega(idBodega);
            return this.ApiOk(movimientos, "Movimientos de la bodega obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimientos por bodega");
            return this.ApiError(ex.Message);
        }
    }
}

// DTO auxiliar para obtener por ID
public class ObtenerMovimientoPorIdDto
{
    public int Id { get; set; }
}