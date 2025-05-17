using ProyectoGraduación.Models;

namespace ProyectoGraduación.IServices;
public interface IProductoService
{
    Task<IEnumerable<Producto>> GetAll();
    Task<Producto> GetById(int id);
    Task Add(Producto producto);
    Task Update(Producto producto);
    Task Delete(int id);
}
