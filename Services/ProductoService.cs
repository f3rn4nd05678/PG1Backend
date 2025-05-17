using ProyectoGraduación.Models;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.IServices;

namespace ProyectoGraduación.Services;
public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repository;

    public ProductoService(IProductoRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Producto>> GetAll() => _repository.GetAll();
    public Task<Producto> GetById(int id) => _repository.GetById(id);
    public Task Add(Producto producto) => _repository.Add(producto);
    public Task Update(Producto producto) => _repository.Update(producto);
    public Task Delete(int id) => _repository.Delete(id);
}
