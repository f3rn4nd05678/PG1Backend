using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.IServices;

public interface IProductoService
{
    Task<IEnumerable<ProductoDto>> GetAll();
    Task<(IEnumerable<ProductoDto> productos, int total)> GetWithFilters(FiltroProductoDto filtro);
    Task<ProductoDto?> GetById(int id);
    Task<ProductoDto?> GetByCodigo(string codigo);
    Task<IEnumerable<ProductoDto>> GetByProveedor(int proveedorId);
    Task<IEnumerable<ProductoDto>> SearchByNombreOrCodigo(string termino);
    Task<IEnumerable<string>> GetCategorias();
    Task<ProductoDto> CreateProducto(CrearProductoDto crearProductoDto);
    Task<ProductoDto> UpdateProducto(int id, ActualizarProductoDto actualizarProductoDto);
    Task DeleteProducto(int id);
    Task<bool> ExisteCodigo(string codigo, int? excludeId = null);
}
