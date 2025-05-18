using ProyectoGraduación.Models;

namespace ProyectoGraduación.IRepositories;
public interface IUsuarioRepository
{
    Task<IEnumerable<Usuario>> GetAll();
    Task<Usuario> GetById(int id);
    Task<Usuario> GetByCorreo(string correo);
    Task Add(Usuario usuario);
    Task Update(Usuario usuario);
    Task Delete(int id);
    Task<bool> ExisteCorreo(string correo);
    Task<IEnumerable<Usuario>> GetByRol(int rolId);
}