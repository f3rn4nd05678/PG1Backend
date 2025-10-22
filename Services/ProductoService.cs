using ProyectoGraduación.Models;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.IServices;

namespace ProyectoGraduación.Services;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _productoRepository;

    public ProductoService(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<IEnumerable<ProductoDto>> GetAll()
    {
        var productos = await _productoRepository.GetAll();
        return productos.Select(MapToDto);
    }

    public async Task<(IEnumerable<ProductoDto> productos, int total)> GetWithFilters(FiltroProductoDto filtro)
    {
        var (productos, total) = await _productoRepository.GetWithFilters(
            filtro.TerminoBusqueda,
            filtro.Categoria,
            filtro.ProveedorId,
            filtro.PrecioMinimo,
            filtro.PrecioMaximo,
            filtro.NumeroPagina,
            filtro.TamanoPagina);

        var productosDto = productos.Select(MapToDto);
        return (productosDto, total);
    }

    public async Task<ProductoDto?> GetById(int id)
    {
        var producto = await _productoRepository.GetById(id);
        return producto == null ? null : MapToDto(producto);
    }

    public async Task<ProductoDto?> GetByCodigo(string codigo)
    {
        var producto = await _productoRepository.GetByCodigo(codigo);
        return producto == null ? null : MapToDto(producto);
    }

    public async Task<IEnumerable<ProductoDto>> GetByProveedor(int proveedorId)
    {
        var productos = await _productoRepository.GetByProveedor(proveedorId);
        return productos.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductoDto>> SearchByNombreOrCodigo(string termino)
    {
        var productos = await _productoRepository.SearchByNombreOrCodigo(termino);
        return productos.Select(MapToDto);
    }

    public async Task<IEnumerable<string>> GetCategorias()
    {
        return await _productoRepository.GetCategorias();
    }

    public async Task<ProductoDto> CreateProducto(CrearProductoDto crearProductoDto)
    {
        // Validar que el código no exista
        if (await _productoRepository.ExisteCodigo(crearProductoDto.Codigo))
        {
            throw new InvalidOperationException($"Ya existe un producto con el código '{crearProductoDto.Codigo}'");
        }

        var producto = new Producto
        {
            Nombre = crearProductoDto.Nombre,
            Codigo = crearProductoDto.Codigo,
            Categoria = crearProductoDto.Categoria,
            Precio = crearProductoDto.Precio,
            StockMinimo = crearProductoDto.StockMinimo,
            ProveedorId = crearProductoDto.ProveedorId
        };

        await _productoRepository.Add(producto);

        // Obtener el producto creado con sus relaciones
        var productoCreado = await _productoRepository.GetById(producto.Id);
        return MapToDto(productoCreado!);
    }

    public async Task<ProductoDto> UpdateProducto(int id, ActualizarProductoDto actualizarProductoDto)
    {
        var producto = await _productoRepository.GetById(id);
        if (producto == null)
        {
            throw new KeyNotFoundException($"No se encontró el producto con ID {id}");
        }

        // Actualizar campos
        producto.Nombre = actualizarProductoDto.Nombre;
        producto.Categoria = actualizarProductoDto.Categoria;
        producto.Precio = actualizarProductoDto.Precio;
        producto.StockMinimo = actualizarProductoDto.StockMinimo;
        producto.ProveedorId = actualizarProductoDto.ProveedorId;

        await _productoRepository.Update(producto);

        // Obtener el producto actualizado con sus relaciones
        var productoActualizado = await _productoRepository.GetById(id);
        return MapToDto(productoActualizado!);
    }

    public async Task DeleteProducto(int id)
    {
        var producto = await _productoRepository.GetById(id);
        if (producto == null)
        {
            throw new KeyNotFoundException($"No se encontró el producto con ID {id}");
        }

        await _productoRepository.Delete(id);
    }

    public async Task<bool> ExisteCodigo(string codigo, int? excludeId = null)
    {
        return await _productoRepository.ExisteCodigo(codigo, excludeId);
    }

    private ProductoDto MapToDto(Producto producto)
    {
        return new ProductoDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Codigo = producto.Codigo,
            Categoria = producto.Categoria,
            Precio = producto.Precio,
            StockMinimo = producto.StockMinimo,
            ProveedorId = producto.ProveedorId,
            ProveedorNombre = producto.Proveedor?.Nombre
        };
    }
}
