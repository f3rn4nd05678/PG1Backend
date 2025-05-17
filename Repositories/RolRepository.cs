using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Models;
using ProyectoGraduación.Data;
using ProyectoGraduación.IRepositories;

namespace ProyectoGraduación.Repositories;

public class RolRepository : IRolRepository
{
    private readonly AppDbContext _context;

    public RolRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Rol>> GetAll() =>
        await _context.Roles.ToListAsync();

    public async Task<Rol> GetById(int id) =>
        await _context.Roles.FindAsync(id);

    public async Task Add(Rol rol)
    {
        _context.Roles.Add(rol);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Rol rol)
    {
        _context.Roles.Update(rol);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var rol = await _context.Roles.FindAsync(id);
        if (rol != null)
        {
            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();
        }
    }
}