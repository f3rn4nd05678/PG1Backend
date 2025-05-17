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

    public async Task<IEnumerable<Producto>> GetAll() =>
        await _context.Productos.ToListAsync();

    public async Task<Producto> GetById(int id) =>
        await _context.Productos.FindAsync(id);

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
}
