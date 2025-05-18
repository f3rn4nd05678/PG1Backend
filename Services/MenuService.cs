using ProyectoGraduación.Models;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Services;

namespace ProyectoGraduación.Services;
public class MenuService : IMenuService
{
    private readonly IMenuRepository _repository;

    public MenuService(IMenuRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<MenuDto>> GetAll()
    {
        var menus = await _repository.GetAll();
        return BuildMenuTree(menus.ToList());
    }

    public async Task<IEnumerable<MenuDto>> GetMenusByRolId(int rolId)
    {
        var menus = await _repository.GetMenusByRolId(rolId);
        return BuildMenuTree(menus.ToList());
    }

    public Task<Menu> GetById(int id) => _repository.GetById(id);
    public Task Add(Menu menu) => _repository.Add(menu);
    public Task Update(Menu menu) => _repository.Update(menu);
    public Task Delete(int id) => _repository.Delete(id);

    private IEnumerable<MenuDto> BuildMenuTree(List<Menu> allMenus)
    {
        // Primero obtenemos los menús padres (sin MenuPadreId)
        var rootMenus = allMenus.Where(m => m.MenuPadreId == null).ToList();

        // Transformamos los menús padres en DTOs y añadimos sus hijos recursivamente
        var menuDtos = rootMenus.Select(m => MapToDto(m, allMenus)).ToList();

        return menuDtos;
    }

    private MenuDto MapToDto(Menu menu, List<Menu> allMenus)
    {
        var menuDto = new MenuDto
        {
            Id = menu.Id,
            Titulo = menu.Titulo,
            Ruta = menu.Ruta ?? "#", 
            Icono = menu.Icono ?? "default",
            MenuPadreId = menu.MenuPadreId,
            Orden = menu.Orden,
            Activo = menu.Activo
        };


        var children = allMenus.Where(m => m.MenuPadreId == menu.Id).OrderBy(m => m.Orden).ToList();


        if (children.Any())
        {
            menuDto.Items = children.Select(child => MapToDto(child, allMenus)).ToList();
        }

        return menuDto;
    }
}