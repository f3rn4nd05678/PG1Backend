using ProyectoGraduación.Models;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.IServices;

namespace ProyectoGraduación.Services;
public class PermisoService : IPermisoService
{
    private readonly IPermisoRepository _repository;

    public PermisoService(IPermisoRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Permiso>> GetAll() => _repository.GetAll();
    public Task<Permiso> GetById(int id) => _repository.GetById(id);
    public Task Add(Permiso permiso) => _repository.Add(permiso);
    public Task Update(Permiso permiso) => _repository.Update(permiso);
    public Task Delete(int id) => _repository.Delete(id);
    public Task<IEnumerable<Permiso>> GetPermisosByRolId(int rolId) => _repository.GetPermisosByRolId(rolId);
    public Task AsignarPermisoARol(int rolId, int permisoId) => _repository.AsignarPermisoARol(rolId, permisoId);
    public Task RemoverPermisoDeRol(int rolId, int permisoId) => _repository.RemoverPermisoDeRol(rolId, permisoId);
}