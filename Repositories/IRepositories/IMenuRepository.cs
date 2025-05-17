using ProyectoGraduación.Models;

namespace ProyectoGraduación.IRepositories;
public interface IMenuRepository
{
    Task<IEnumerable<Menu>> GetAll();
    Task<IEnumerable<Menu>> GetMenusByRolId(int rolId);
    Task<Menu> GetById(int id);
    Task Add(Menu menu);
    Task Update(Menu menu);
    Task Delete(int id);
}