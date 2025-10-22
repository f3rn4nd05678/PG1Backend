using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Models;
using ProyectoGraduación.Data;
using ProyectoGraduación.IRepositories;

namespace ProyectoGraduación.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    // Deja UN SOLO GetAll
    public async Task<IEnumerable<Usuario>> GetAll() =>
        await _context.Usuarios
            .Include(u => u.Rol)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Usuario?> GetByCorreo(string correo)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Correo == correo);
    }

    public async Task<Usuario?> GetById(int id)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task Add(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExisteCorreo(string correo) =>
        await _context.Usuarios.AnyAsync(u => u.Correo == correo);

    public async Task<IEnumerable<Usuario>> GetByRol(int rolId)
    {
        return await _context.Usuarios
            .Where(u => u.RolId == rolId)
            .Include(u => u.Rol)
            .AsNoTracking()
            .ToListAsync();
    }
}
