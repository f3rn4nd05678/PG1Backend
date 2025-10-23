using ProyectoGraduación.Models;

namespace ProyectoGraduación.Repositories.IRepositories;

public interface IProductoRepository
{
    // Métodos principales
    Task<IEnumerable<Producto>> GetAll();
    Task<(IEnumerable<Producto>, int)> GetWithFilters(
        string? terminoBusqueda,
        int? categoriaId,
        int? proveedorId,
        decimal? precioMinimo,
        decimal? precioMaximo,
        int numeroPagina,
        int tamanoPagina);
    Task<Producto?> GetById(int id);
    Task<Producto?> GetByCodigo(string codigo);
    Task<IEnumerable<Producto>> GetByProveedor(int proveedorId);
    Task<IEnumerable<Producto>> SearchByNombreOrCodigo(string termino);
    Task<Producto> Create(Producto producto);
    Task<bool> Update(Producto producto);
    Task<bool> Delete(int id);
    Task<bool> ExisteCodigo(string codigo, int? idExcluir = null);
}