using ProyectoGraduación.Models;

namespace ProyectoGraduación.IRepositories;

public interface IProductoRepository
{
    Task<IEnumerable<Producto>> GetAll();
    Task<(IEnumerable<Producto> productos, int total)> GetWithFilters(
        string? terminoBusqueda,
        string? categoria,
        int? proveedorId,
        decimal? precioMinimo,
        decimal? precioMaximo,
        int numeroPagina,
        int tamanoPagina);
    Task<Producto?> GetById(int id);
    Task<Producto?> GetByCodigo(string codigo);
    Task<IEnumerable<Producto>> GetByProveedor(int proveedorId);
    Task<IEnumerable<Producto>> SearchByNombreOrCodigo(string termino);
    Task<IEnumerable<string>> GetCategorias();
    Task Add(Producto producto);
    Task Update(Producto producto);
    Task Delete(int id);
    Task<bool> ExisteCodigo(string codigo, int? excludeId = null);
}
