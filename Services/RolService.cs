using ProyectoGraduación.Models;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.IServices;

namespace ProyectoGraduación.Services;
public class RolService : IRolService
{
    private readonly IRolRepository _repository;

    public RolService(IRolRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Rol>> GetAll() => _repository.GetAll();
    public Task<Rol> GetById(int id) => _repository.GetById(id);
    public Task Add(Rol rol) => _repository.Add(rol);
    public Task Update(Rol rol) => _repository.Update(rol);
    public Task Delete(int id) => _repository.Delete(id);
}