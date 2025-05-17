using ProyectoGraduación.Models;

namespace ProyectoGraduación.IServices;
public interface IRolService
{
    Task<IEnumerable<Rol>> GetAll();
    Task<Rol> GetById(int id);
    Task Add(Rol rol);
    Task Update(Rol rol);
    Task Delete(int id);
}
