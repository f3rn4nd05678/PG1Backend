using ProyectoGraduación.Models;

namespace ProyectoGraduación.IRepositories;
public interface IProductoRepository
{
    Task<IEnumerable<Producto>> GetAll();
    Task<Producto> GetById(int id);
    Task Add(Producto producto);
    Task Update(Producto producto);
    Task Delete(int id);
}
