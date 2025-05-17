using ProyectoGraduación.Models;
using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.IServices;
public interface IMenuService
{
    Task<IEnumerable<MenuDto>> GetAll();
    Task<IEnumerable<MenuDto>> GetMenusByRolId(int rolId);
    Task<Menu> GetById(int id);
    Task Add(Menu menu);
    Task Update(Menu menu);
    Task Delete(int id);
}