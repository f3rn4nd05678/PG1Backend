using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Data;
using ProyectoGraduación.Models;
using ProyectoGraduación.Repositories.IRepositories;

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
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Producto>, int)> GetWithFilters(
        string? terminoBusqueda,
        int? categoriaId,
        int? proveedorId,
        decimal? precioMinimo,
        decimal? precioMaximo,
        int numeroPagina,
        int tamanoPagina)
    {
        var query = _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .AsQueryable();

        // Filtros
        if (!string.IsNullOrEmpty(terminoBusqueda))
        {
            query = query.Where(p =>
                p.Nombre.Contains(terminoBusqueda) ||
                p.Codigo.Contains(terminoBusqueda));
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(p => p.CategoriaId == categoriaId.Value);
        }

        if (proveedorId.HasValue)
        {
            query = query.Where(p => p.ProveedorId == proveedorId.Value);
        }

        if (precioMinimo.HasValue)
        {
            query = query.Where(p => p.Precio >= precioMinimo.Value);
        }

        if (precioMaximo.HasValue)
        {
            query = query.Where(p => p.Precio <= precioMaximo.Value);
        }

        var total = await query.CountAsync();

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
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Producto?> GetByCodigo(string codigo)
    {
        return await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync(p => p.Codigo == codigo);
    }

    public async Task<IEnumerable<Producto>> GetByProveedor(int proveedorId)
    {
        return await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .Where(p => p.ProveedorId == proveedorId)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Producto>> SearchByNombreOrCodigo(string termino)
    {
        return await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .Where(p => p.Nombre.Contains(termino) || p.Codigo.Contains(termino))
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<Producto> Create(Producto producto)
    {
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        // Recargar el producto con sus relaciones
        await _context.Entry(producto).Reference(p => p.Categoria).LoadAsync();
        if (producto.ProveedorId.HasValue)
        {
            await _context.Entry(producto).Reference(p => p.Proveedor).LoadAsync();
        }

        return producto;
    }

    public async Task<bool> Update(Producto producto)
    {
        _context.Productos.Update(producto);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Delete(int id)
    {
        var producto = await GetById(id);
        if (producto == null)
            return false;

        _context.Productos.Remove(producto);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ExisteCodigo(string codigo, int? idExcluir = null)
    {
        return await _context.Productos
            .AnyAsync(p => p.Codigo == codigo && (idExcluir == null || p.Id != idExcluir));
    }
}