using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Models;
using ProyectoGraduación.Data;
using ProyectoGraduación.IRepositories;

namespace ProyectoGraduación.Repositories;

public class ProveedorRepository : IProveedorRepository
{
    private readonly AppDbContext _context;

    public ProveedorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Proveedor>> GetAll()
    {
        return await _context.Proveedores
            .Include(p => p.Productos)
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Proveedor> proveedores, int total)> GetWithFilters(
        string? terminoBusqueda,
        bool? activo,
        int numeroPagina,
        int tamanoPagina)
    {
        var query = _context.Proveedores
            .Include(p => p.Productos)
            .AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(terminoBusqueda))
        {
            terminoBusqueda = terminoBusqueda.ToLower();
            query = query.Where(p =>
                p.Nombre.ToLower().Contains(terminoBusqueda) ||
                (p.Contacto != null && p.Contacto.ToLower().Contains(terminoBusqueda)) ||
                (p.Nit != null && p.Nit.Contains(terminoBusqueda)));
        }

        if (activo.HasValue)
        {
            query = query.Where(p => p.Activo == activo.Value);
        }

        // Contar total antes de paginar
        var total = await query.CountAsync();

        // Aplicar paginación
        var proveedores = await query
            .OrderBy(p => p.Nombre)
            .Skip((numeroPagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return (proveedores, total);
    }

    public async Task<Proveedor?> GetById(int id)
    {
        return await _context.Proveedores
            .Include(p => p.Productos)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Proveedor>> SearchByNombre(string termino)
    {
        termino = termino.ToLower();
        return await _context.Proveedores
            .Where(p => p.Activo && p.Nombre.ToLower().Contains(termino))
            .OrderBy(p => p.Nombre)
            .Take(20)
            .ToListAsync();
    }

    public async Task Add(Proveedor proveedor)
    {
        _context.Proveedores.Add(proveedor);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Proveedor proveedor)
    {
        _context.Proveedores.Update(proveedor);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor != null)
        {
            // Soft delete: marcar como inactivo en lugar de eliminar
            proveedor.Activo = false;
            _context.Proveedores.Update(proveedor);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExisteNombre(string nombre, int? excludeId = null)
    {
        var query = _context.Proveedores.Where(p => p.Nombre == nombre);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
