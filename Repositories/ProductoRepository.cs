using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Models;
using ProyectoGraduación.Data;
using ProyectoGraduación.IRepositories;

namespace ProyectoGraduación.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly AppDbContext _context;

    public ProductoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Producto>> GetAll()
    {
        return await _context.Productos
            .Include(p => p.Proveedor)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Producto> productos, int total)> GetWithFilters(
        string? terminoBusqueda,
        string? categoria,
        int? proveedorId,
        decimal? precioMinimo,
        decimal? precioMaximo,
        int numeroPagina,
        int tamanoPagina)
    {
        var query = _context.Productos
            .Include(p => p.Proveedor)
            .AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(terminoBusqueda))
        {
            terminoBusqueda = terminoBusqueda.ToLower();
            query = query.Where(p =>
                p.Nombre.ToLower().Contains(terminoBusqueda) ||
                p.Codigo.ToLower().Contains(terminoBusqueda));
        }

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            query = query.Where(p => p.Categoria == categoria);
        }

        if (proveedorId.HasValue)
        {
            query = query.Where(p => p.ProveedorId == proveedorId);
        }

        if (precioMinimo.HasValue)
        {
            query = query.Where(p => p.Precio >= precioMinimo);
        }

        if (precioMaximo.HasValue)
        {
            query = query.Where(p => p.Precio <= precioMaximo);
        }

        // Contar total antes de paginar
        var total = await query.CountAsync();

        // Aplicar paginación
        var productos = await query
            .OrderBy(p => p.Nombre)
            .Skip((numeroPagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return (productos, total);
    }

    public async Task<Producto?> GetById(int id)
    {
        return await _context.Productos
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Producto?> GetByCodigo(string codigo)
    {
        return await _context.Productos
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync(p => p.Codigo == codigo);
    }

    public async Task<IEnumerable<Producto>> GetByProveedor(int proveedorId)
    {
        return await _context.Productos
            .Include(p => p.Proveedor)
            .Where(p => p.ProveedorId == proveedorId)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Producto>> SearchByNombreOrCodigo(string termino)
    {
        termino = termino.ToLower();
        return await _context.Productos
            .Include(p => p.Proveedor)
            .Where(p =>
                p.Nombre.ToLower().Contains(termino) ||
                p.Codigo.ToLower().Contains(termino))
            .OrderBy(p => p.Nombre)
            .Take(20)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetCategorias()
    {
        return await _context.Productos
            .Where(p => !string.IsNullOrEmpty(p.Categoria))
            .Select(p => p.Categoria!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public async Task Add(Producto producto)
    {
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Producto producto)
    {
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto != null)
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExisteCodigo(string codigo, int? excludeId = null)
    {
        var query = _context.Productos.Where(p => p.Codigo == codigo);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
