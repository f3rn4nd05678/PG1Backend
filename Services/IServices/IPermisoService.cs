using ProyectoGraduación.Models;

namespace ProyectoGraduación.IServices;
public interface IPermisoService
{
    Task<IEnumerable<Permiso>> GetAll();
    Task<Permiso> GetById(int id);
    Task Add(Permiso permiso);
    Task Update(Permiso permiso);
    Task Delete(int id);
    Task<IEnumerable<Permiso>> GetPermisosByRolId(int rolId);
    Task AsignarPermisoARol(int rolId, int permisoId);
    Task RemoverPermisoDeRol(int rolId, int permisoId);
}