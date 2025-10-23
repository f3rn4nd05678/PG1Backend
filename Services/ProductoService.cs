using ProyectoGraduación.DTOs;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.Models;
using ProyectoGraduación.Repositories.IRepositories;
using ProyectoGraduación.Services.IServices;

namespace ProyectoGraduación.Services;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _productoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IProveedorRepository _proveedorRepository;

    public ProductoService(
        IProductoRepository productoRepository,
        ICategoriaRepository categoriaRepository,
        IProveedorRepository proveedorRepository)
    {
        _productoRepository = productoRepository;
        _categoriaRepository = categoriaRepository;
        _proveedorRepository = proveedorRepository;
    }

    // Métodos principales
    public async Task<(IEnumerable<ProductoDto> productos, int total)> ObtenerProductosAsync(FiltroProductoDto filtro)
    {
        var productos = await _productoRepository.GetWithFilters(
            filtro.TerminoBusqueda,
            filtro.CategoriaId,
            filtro.ProveedorId,
            filtro.PrecioMinimo,
            filtro.PrecioMaximo,
            filtro.NumeroPagina,
            filtro.TamanoPagina
        );

        var productosDto = productos.Item1.Select(p => MapToDto(p)).ToList();
        return (productosDto, productos.Item2);
    }

    public async Task<ProductoDto?> ObtenerProductoPorIdAsync(int id)
    {
        var producto = await _productoRepository.GetById(id);
        return producto != null ? MapToDto(producto) : null;
    }

    public async Task<ProductoDto> CrearProductoAsync(CrearProductoDto dto)
    {
        // Validar que la categoría existe
        var categoria = await _categoriaRepository.ObtenerPorIdAsync(dto.CategoriaId);
        if (categoria == null)
            throw new InvalidOperationException("La categoría especificada no existe");

        // Validar proveedor si se especifica
        if (dto.ProveedorId.HasValue)
        {
            var proveedor = await _proveedorRepository.GetById(dto.ProveedorId.Value);
            if (proveedor == null)
                throw new InvalidOperationException("El proveedor especificado no existe");
        }

        var producto = new Producto
        {
            Nombre = dto.Nombre,
            // Codigo se genera automáticamente por trigger de BD
            CategoriaId = dto.CategoriaId,
            Precio = dto.Precio,
            StockMinimo = dto.StockMinimo,
            ProveedorId = dto.ProveedorId
        };

        var productoCreado = await _productoRepository.Create(producto);

        // Recargar para obtener el código generado y las relaciones
        var productoCompleto = await _productoRepository.GetById(productoCreado.Id);

        return MapToDto(productoCompleto!);
    }

    public async Task<bool> ActualizarProductoAsync(int id, ActualizarProductoDto dto)
    {
        var producto = await _productoRepository.GetById(id);
        if (producto == null)
            return false;

        // Validar categoría
        var categoria = await _categoriaRepository.ObtenerPorIdAsync(dto.CategoriaId);
        if (categoria == null)
            throw new InvalidOperationException("La categoría especificada no existe");

        // Validar proveedor si se especifica
        if (dto.ProveedorId.HasValue)
        {
            var proveedor = await _proveedorRepository.GetById(dto.ProveedorId.Value);
            if (proveedor == null)
                throw new InvalidOperationException("El proveedor especificado no existe");
        }

        // NO permitir cambiar el código
        producto.Nombre = dto.Nombre;
        producto.CategoriaId = dto.CategoriaId;
        producto.Precio = dto.Precio;
        producto.StockMinimo = dto.StockMinimo;
        producto.ProveedorId = dto.ProveedorId;

        return await _productoRepository.Update(producto);
    }

    public async Task<bool> EliminarProductoAsync(int id)
    {
        return await _productoRepository.Delete(id);
    }

    public async Task<bool> ExisteCodigoAsync(string codigo)
    {
        return await _productoRepository.ExisteCodigo(codigo);
    }

    // Métodos de compatibilidad (alias)
    public async Task<IEnumerable<ProductoDto>> GetAll()
    {
        var productos = await _productoRepository.GetAll();
        return productos.Select(p => MapToDto(p)).ToList();
    }

    public async Task<(IEnumerable<ProductoDto>, int)> GetWithFilters(FiltroProductoDto filtro)
    {
        return await ObtenerProductosAsync(filtro);
    }

    public async Task<ProductoDto?> GetById(int id)
    {
        return await ObtenerProductoPorIdAsync(id);
    }

    public async Task<ProductoDto?> GetByCodigo(string codigo)
    {
        var producto = await _productoRepository.GetByCodigo(codigo);
        return producto != null ? MapToDto(producto) : null;
    }

    public async Task<IEnumerable<ProductoDto>> GetByProveedor(int proveedorId)
    {
        var productos = await _productoRepository.GetByProveedor(proveedorId);
        return productos.Select(p => MapToDto(p)).ToList();
    }

    public async Task<IEnumerable<ProductoDto>> SearchByNombreOrCodigo(string termino)
    {
        var productos = await _productoRepository.SearchByNombreOrCodigo(termino);
        return productos.Select(p => MapToDto(p)).ToList();
    }

    public async Task<IEnumerable<CategoriaDto>> GetCategorias()
    {
        var categorias = await _categoriaRepository.ObtenerTodosAsync();
        return categorias.Select(c => new CategoriaDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            CodigoPrefijo = c.CodigoPrefijo,
            Descripcion = c.Descripcion,
            Activo = c.Activo
        }).ToList();
    }

    public async Task<ProductoDto> CreateProducto(CrearProductoDto dto)
    {
        return await CrearProductoAsync(dto);
    }

    public async Task<bool> UpdateProducto(int id, ActualizarProductoDto dto)
    {
        return await ActualizarProductoAsync(id, dto);
    }

    public async Task<bool> DeleteProducto(int id)
    {
        return await EliminarProductoAsync(id);
    }

    public async Task<bool> ExisteCodigo(string codigo, int? idExcluir = null)
    {
        if (idExcluir.HasValue)
        {
            var producto = await _productoRepository.GetById(idExcluir.Value);
            if (producto != null && producto.Codigo == codigo)
                return false; // El código pertenece al mismo producto
        }
        return await ExisteCodigoAsync(codigo);
    }

    // Helper privado para mapear
    private ProductoDto MapToDto(Producto p)
    {
        return new ProductoDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Codigo = p.Codigo,
            CategoriaId = p.CategoriaId,
            CategoriaNombre = p.Categoria?.Nombre ?? "",
            Precio = p.Precio,
            StockMinimo = p.StockMinimo,
            ProveedorId = p.ProveedorId,
            ProveedorNombre = p.Proveedor?.Nombre
        };
    }
}