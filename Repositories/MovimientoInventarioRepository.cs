using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Data;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.Models;

namespace ProyectoGraduación.Repositories;

public class MovimientoInventarioRepository : IMovimientoInventarioRepository
{
    private readonly AppDbContext _context;

    public MovimientoInventarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MovimientoInventario>> GetAll()
    {
        return await _context.MovimientosInventario
            .Include(m => m.Producto)
            .Include(m => m.Bodega)
            .Include(m => m.Usuario)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    public async Task<MovimientoInventario?> GetById(int id)
    {
        return await _context.MovimientosInventario
            .Include(m => m.Producto)
            .Include(m => m.Bodega)
            .Include(m => m.Usuario)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<(IEnumerable<MovimientoInventario>, int)> GetWithFilters(FiltroMovimientoInventarioDto filtro)
    {
        var query = _context.MovimientosInventario
            .Include(m => m.Producto)
                .ThenInclude(p => p.Categoria)
            .Include(m => m.Bodega)
            .Include(m => m.Usuario)
            .AsQueryable();

        // Filtro por búsqueda (producto o bodega)
        if (!string.IsNullOrWhiteSpace(filtro.TerminoBusqueda))
        {
            var termino = filtro.TerminoBusqueda.ToLower();
            query = query.Where(m =>
                m.Producto.Nombre.ToLower().Contains(termino) ||
                m.Producto.Codigo.ToLower().Contains(termino) ||
                m.Bodega.Nombre.ToLower().Contains(termino) ||
                (m.Referencia != null && m.Referencia.ToLower().Contains(termino))
            );
        }

        // Filtro por bodega
        if (filtro.IdBodega.HasValue)
        {
            query = query.Where(m => m.IdBodega == filtro.IdBodega.Value);
        }

        // Filtro por producto
        if (filtro.IdProducto.HasValue)
        {
            query = query.Where(m => m.IdProducto == filtro.IdProducto.Value);
        }

        // Filtro por tipo
        if (!string.IsNullOrWhiteSpace(filtro.Tipo))
        {
            query = query.Where(m => m.Tipo == filtro.Tipo);
        }

        // Filtro por rango de fechas
        if (filtro.FechaDesde.HasValue)
        {
            query = query.Where(m => m.Fecha >= filtro.FechaDesde.Value);
        }

        if (filtro.FechaHasta.HasValue)
        {
            var fechaHastaFin = filtro.FechaHasta.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(m => m.Fecha <= fechaHastaFin);
        }

        // Contar total
        var total = await query.CountAsync();

        // Ordenamiento
        query = filtro.OrdenarPor?.ToLower() switch
        {
            "producto" => filtro.Descendente
                ? query.OrderByDescending(m => m.Producto.Nombre)
                : query.OrderBy(m => m.Producto.Nombre),
            "bodega" => filtro.Descendente
                ? query.OrderByDescending(m => m.Bodega.Nombre)
                : query.OrderBy(m => m.Bodega.Nombre),
            "cantidad" => filtro.Descendente
                ? query.OrderByDescending(m => m.Cantidad)
                : query.OrderBy(m => m.Cantidad),
            _ => filtro.Descendente
                ? query.OrderByDescending(m => m.Fecha)
                : query.OrderBy(m => m.Fecha)
        };

        // Paginación
        if (filtro.Pagina > 0 && filtro.ElementosPorPagina > 0)
        {
            query = query
                .Skip((filtro.Pagina - 1) * filtro.ElementosPorPagina)
                .Take(filtro.ElementosPorPagina);
        }

        var movimientos = await query.ToListAsync();

        return (movimientos, total);
    }

    public async Task<MovimientoInventario> Create(MovimientoInventario movimiento)
    {
        movimiento.Fecha = DateTime.UtcNow;

        _context.MovimientosInventario.Add(movimiento);
        await _context.SaveChangesAsync();

        return movimiento;
    }

    public async Task<IEnumerable<MovimientoInventario>> GetByProducto(int idProducto)
    {
        return await _context.MovimientosInventario
            .Include(m => m.Bodega)
            .Include(m => m.Usuario)
            .Where(m => m.IdProducto == idProducto)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    public async Task<IEnumerable<MovimientoInventario>> GetByBodega(int idBodega)
    {
        return await _context.MovimientosInventario
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .Where(m => m.IdBodega == idBodega)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    public async Task<IEnumerable<MovimientoInventario>> GetByFechaRango(DateTime fechaDesde, DateTime fechaHasta)
    {
        return await _context.MovimientosInventario
            .Include(m => m.Producto)
            .Include(m => m.Bodega)
            .Include(m => m.Usuario)
            .Where(m => m.Fecha >= fechaDesde && m.Fecha <= fechaHasta)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }
}