using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Models;
using ProyectoGraduación.Data;
using ProyectoGraduación.IRepositories;

namespace ProyectoGraduación.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly AppDbContext _context;

    public MenuRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Menu>> GetAll() =>
        await _context.Menus.Include(m => m.MenuPadre).ToListAsync();

    public async Task<IEnumerable<Menu>> GetMenusByRolId(int rolId)
    {
        // Obtener los permisos asociados al rol
        var permisosIds = await _context.RolesPermisos
            .Where(rp => rp.RolId == rolId)
            .Select(rp => rp.PermisoId)
            .ToListAsync();

        // Obtener los menús asociados a esos permisos
        return await _context.Menus
            .Where(m => permisosIds.Contains(m.PermisoId) && m.Activo)
            .OrderBy(m => m.Orden)
            .ToListAsync();
    }

    public async Task<Menu> GetById(int id) =>
        await _context.Menus.FindAsync(id);

    public async Task Add(Menu menu)
    {
        _context.Menus.Add(menu);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Menu menu)
    {
        _context.Menus.Update(menu);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu != null)
        {
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
        }
    }
}
