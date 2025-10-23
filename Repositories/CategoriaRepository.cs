using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Data;
using ProyectoGraduación.Models;
using ProyectoGraduación.Repositories.IRepositories;

namespace ProyectoGraduación.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _context;

    public CategoriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Categoria>> ObtenerTodosAsync()
    {
        return await _context.Categorias
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<Categoria?> ObtenerPorIdAsync(int id)
    {
        return await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Categoria> CrearAsync(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        return categoria;
    }

    public async Task<bool> ActualizarAsync(Categoria categoria)
    {
        _context.Categorias.Update(categoria);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var categoria = await ObtenerPorIdAsync(id);
        if (categoria == null)
            return false;

        // Soft delete
        categoria.Activo = false;
        return await ActualizarAsync(categoria);
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null)
    {
        return await _context.Categorias
            .AnyAsync(c => c.Nombre == nombre && (idExcluir == null || c.Id != idExcluir));
    }

    public async Task<bool> ExistePrefijoAsync(string prefijo, int? idExcluir = null)
    {
        return await _context.Categorias
            .AnyAsync(c => c.CodigoPrefijo == prefijo && (idExcluir == null || c.Id != idExcluir));
    }
}