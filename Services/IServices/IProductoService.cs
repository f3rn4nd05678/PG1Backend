using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.Services.IServices;

public interface IProductoService
{
    Task<(IEnumerable<ProductoDto> productos, int total)> ObtenerProductosAsync(FiltroProductoDto filtro);
    Task<ProductoDto?> ObtenerProductoPorIdAsync(int id);
    Task<ProductoDto> CrearProductoAsync(CrearProductoDto dto);
    Task<bool> ActualizarProductoAsync(int id, ActualizarProductoDto dto);
    Task<bool> EliminarProductoAsync(int id);
    Task<bool> ExisteCodigoAsync(string codigo);
    Task<IEnumerable<ProductoDto>> GetAll();
    Task<(IEnumerable<ProductoDto>, int)> GetWithFilters(FiltroProductoDto filtro);
    Task<ProductoDto?> GetById(int id);
    Task<ProductoDto?> GetByCodigo(string codigo);
    Task<IEnumerable<ProductoDto>> GetByProveedor(int proveedorId);
    Task<IEnumerable<ProductoDto>> SearchByNombreOrCodigo(string termino);
    Task<IEnumerable<CategoriaDto>> GetCategorias();
    Task<bool> ExisteCodigo(string codigo, int? idExcluir = null);
    Task<ProductoDto> CreateProducto(CrearProductoDto dto);
    Task<ProductoDto> UpdateProducto(int id, ActualizarProductoDto dto);
    Task DeleteProducto(int id);
}