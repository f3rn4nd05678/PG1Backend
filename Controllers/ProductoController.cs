using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Extensions;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductoController : ControllerBase
{
    private readonly IProductoService _productoService;
    private readonly ILogger<ProductoController> _logger;

    public ProductoController(IProductoService productoService, ILogger<ProductoController> logger)
    {
        _productoService = productoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todos los productos
    /// </summary>
    [HttpGet("listar")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta,Bodega")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var productos = await _productoService.GetAll();
            return this.ApiOk(productos, "Productos obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los productos");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener productos con filtros y paginación
    /// </summary>
    [HttpPost("listar")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta,Bodega")]
    public async Task<IActionResult> GetWithFilters([FromBody] FiltroProductoDto? filtro = null)
    {
        try
        {
            filtro ??= new FiltroProductoDto();

            var (productos, total) = await _productoService.GetWithFilters(filtro);

            var response = new
            {
                Productos = productos,
                Total = total,
                Pagina = filtro.NumeroPagina,
                TamanoPagina = filtro.TamanoPagina,
                TotalPaginas = (int)Math.Ceiling(total / (double)filtro.TamanoPagina)
            };

            return this.ApiOk(response, "Productos obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos con filtros");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener producto por ID
    /// </summary>
    [HttpPost("obtener")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta,Bodega")]
    public async Task<IActionResult> GetById([FromBody] IdRequest request)
    {
        try
        {
            var producto = await _productoService.GetById(request.Id);
            if (producto == null)
            {
                return this.ApiNotFound($"No se encontró el producto con ID {request.Id}");
            }

            return this.ApiOk(producto, "Producto obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto por ID {Id}", request.Id);
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Buscar productos por nombre o código
    /// </summary>
    [HttpPost("buscar")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta,Bodega")]
    public async Task<IActionResult> Search([FromBody] SearchRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Termino))
            {
                return this.ApiError("El término de búsqueda es requerido");
            }

            var productos = await _productoService.SearchByNombreOrCodigo(request.Termino);
            return this.ApiOk(productos, "Búsqueda completada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar productos");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener productos por proveedor
    /// </summary>
    [HttpGet("por-proveedor/{proveedorId}")]
    [Authorize(Roles = "Administrador,Vendedor,Bodega")]
    public async Task<IActionResult> GetByProveedor(int proveedorId)
    {
        try
        {
            var productos = await _productoService.GetByProveedor(proveedorId);
            return this.ApiOk(productos, "Productos del proveedor obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos del proveedor {ProveedorId}", proveedorId);
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener lista de categorías
    /// </summary>
    [HttpGet("categorias")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta,Bodega")]
    public async Task<IActionResult> GetCategorias()
    {
        try
        {
            var categorias = await _productoService.GetCategorias();
            return this.ApiOk(categorias, "Categorías obtenidas correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorías");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Crear nuevo producto
    /// </summary>
    [HttpPost("crear")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> Create([FromBody] CrearProductoDto crearProductoDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return this.ApiError("Datos inválidos", 400);
            }

            var producto = await _productoService.CreateProducto(crearProductoDto);
            return this.ApiCreated(producto, "Producto creado exitosamente");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al crear producto");
            return this.ApiError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Actualizar producto existente
    /// </summary>
    [HttpPost("actualizar")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> Update([FromBody] UpdateProductoRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return this.ApiError("Datos inválidos", 400);
            }

            var producto = await _productoService.UpdateProducto(request.Id, request.Datos);
            return this.ApiOk(producto, "Producto actualizado exitosamente");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Producto no encontrado");
            return this.ApiNotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar producto");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Eliminar producto
    /// </summary>
    [HttpPost("eliminar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete([FromBody] IdRequest request)
    {
        try
        {
            await _productoService.DeleteProducto(request.Id);
            return this.ApiOk("Producto eliminado exitosamente");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Producto no encontrado");
            return this.ApiNotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Validar si un código ya existe
    /// </summary>
    [HttpPost("validar-codigo")]
    [Authorize(Roles = "Administrador,Bodega")]
    public async Task<IActionResult> ValidarCodigo([FromBody] ValidarCodigoRequest request)
    {
        try
        {
            var existe = await _productoService.ExisteCodigo(request.Codigo, request.ExcluirId);
            return this.ApiOk(new { Existe = existe }, existe ? "El código ya existe" : "El código está disponible");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar código");
            return this.ApiError(ex.Message);
        }
    }
}

// Clases auxiliares para requests
public class IdRequest
{
    public int Id { get; set; }
}

public class SearchRequest
{
    public string Termino { get; set; } = string.Empty;
}

public class UpdateProductoRequest
{
    public int Id { get; set; }
    public ActualizarProductoDto Datos { get; set; } = null!;
}

public class ValidarCodigoRequest
{
    public string Codigo { get; set; } = string.Empty;
    public int? ExcluirId { get; set; }
}