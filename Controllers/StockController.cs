using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.Extensions;
using ProyectoGraduación.IServices;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;
    private readonly ILogger<StockController> _logger;

    public StockController(
        IStockService stockService,
        ILogger<StockController> logger)
    {
        _stockService = stockService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todo el stock
    /// </summary>
    [HttpGet("listar")]
    [Authorize(Roles = "Administrador,Bodega,Vendedor,Punto de venta")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var stocks = await _stockService.GetAll();
            return this.ApiOk(stocks, "Stock obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener stock");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener stock con filtros y paginación
    /// </summary>
    [HttpPost("listar")]
    [Authorize(Roles = "Administrador,Bodega,Vendedor,Punto de venta")]
    public async Task<IActionResult> GetWithFilters([FromBody] FiltroStockDto? filtro = null)
    {
        try
        {
            filtro ??= new FiltroStockDto();

            if (filtro.ElementosPorPagina <= 0)
                filtro.ElementosPorPagina = 20;

            var (stocks, total) = await _stockService.GetWithFilters(filtro);

            var response = new ListadoStockResponseDto
            {
                Stocks = stocks.ToList(),
                Total = total,
                Pagina = filtro.Pagina,
                ElementosPorPagina = filtro.ElementosPorPagina,
                TotalPaginas = (int)Math.Ceiling((double)total / filtro.ElementosPorPagina)
            };

            return this.ApiOk(response, "Stock obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener stock con filtros");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener stock por ID
    /// </summary>
    [HttpPost("obtener")]
    [Authorize(Roles = "Administrador,Bodega,Vendedor,Punto de venta")]
    public async Task<IActionResult> GetById([FromBody] ObtenerStockPorIdDto request)
    {
        try
        {
            var stock = await _stockService.GetById(request.Id);
            if (stock == null)
                return this.ApiNotFound("Stock no encontrado");

            return this.ApiOk(stock, "Stock obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener stock por ID");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener stock de un producto en una bodega específica
    /// </summary>
    [HttpPost("producto-bodega")]
    [Authorize(Roles = "Administrador,Bodega,Vendedor,Punto de venta")]
    public async Task<IActionResult> GetByProductoYBodega([FromBody] ObtenerStockProductoBodegaDto request)
    {
        try
        {
            var stock = await _stockService.GetByProductoYBodega(request.IdProducto, request.IdBodega);
            if (stock == null)
                return this.ApiNotFound("No se encontró stock para este producto en la bodega especificada");

            return this.ApiOk(stock, "Stock obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener stock por producto y bodega");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener productos con stock bajo (alertas)
    /// </summary>
    [HttpGet("alertas")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> GetStockBajo()
    {
        try
        {
            var stocks = await _stockService.GetStockBajo();
            return this.ApiOk(stocks, "Alertas de stock obtenidas correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener alertas de stock");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener stock por bodega
    /// </summary>
    [HttpGet("bodega/{idBodega}")]
    [Authorize(Roles = "Administrador,Bodega,Vendedor,Punto de venta")]
    public async Task<IActionResult> GetByBodega(int idBodega)
    {
        try
        {
            var stocks = await _stockService.GetByBodega(idBodega);
            return this.ApiOk(stocks, "Stock de la bodega obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener stock por bodega");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener stock por producto (en todas las bodegas)
    /// </summary>
    [HttpGet("producto/{idProducto}")]
    [Authorize(Roles = "Administrador,Bodega,Vendedor,Punto de venta")]
    public async Task<IActionResult> GetByProducto(int idProducto)
    {
        try
        {
            var stocks = await _stockService.GetByProducto(idProducto);
            return this.ApiOk(stocks, "Stock del producto obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener stock por producto");
            return this.ApiError(ex.Message);
        }
    }
}

// DTOs auxiliares
public class ObtenerStockPorIdDto
{
    public int Id { get; set; }
}

public class ObtenerStockProductoBodegaDto
{
    public int IdProducto { get; set; }
    public int IdBodega { get; set; }
}