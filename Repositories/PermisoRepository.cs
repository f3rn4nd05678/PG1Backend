using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Models;
using ProyectoGraduación.Data;
using ProyectoGraduación.IRepositories;

namespace ProyectoGraduación.Repositories;

public class PermisoRepository : IPermisoRepository
{
    private readonly AppDbContext _context;

    public PermisoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Permiso>> GetAll() =>
        await _context.Permisos.ToListAsync();

    public async Task<Permiso> GetById(int id) =>
        await _context.Permisos.FindAsync(id);

    public async Task Add(Permiso permiso)
    {
        _context.Permisos.Add(permiso);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Permiso permiso)
    {
        _context.Permisos.Update(permiso);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var permiso = await _context.Permisos.FindAsync(id);
        if (permiso != null)
        {
            _context.Permisos.Remove(permiso);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Permiso>> GetPermisosByRolId(int rolId)
    {
        return await _context.RolesPermisos
            .Where(rp => rp.RolId == rolId)
            .Include(rp => rp.Permiso)
            .Select(rp => rp.Permiso)
            .ToListAsync();
    }

    public async Task AsignarPermisoARol(int rolId, int permisoId)
    {
        // Verificar si ya existe la relación
        bool existe = await _context.RolesPermisos
            .AnyAsync(rp => rp.RolId == rolId && rp.PermisoId == permisoId);

        if (!existe)
        {
            _context.RolesPermisos.Add(new RolPermiso
            {
                RolId = rolId,
                PermisoId = permisoId
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoverPermisoDeRol(int rolId, int permisoId)
    {
        var rolPermiso = await _context.RolesPermisos
            .FirstOrDefaultAsync(rp => rp.RolId == rolId && rp.PermisoId == permisoId);

        if (rolPermiso != null)
        {
            _context.RolesPermisos.Remove(rolPermiso);
            await _context.SaveChangesAsync();
        }
    }
}