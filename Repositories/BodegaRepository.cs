using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Data;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.Models;

namespace ProyectoGraduación.Repositories;

public class BodegaRepository : IBodegaRepository
{
    private readonly AppDbContext _context;

    public BodegaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Bodega>> GetAll()
    {
        return await _context.Bodegas
            .Where(b => b.Activa)
            .Include(b => b.Stocks)
            .OrderBy(b => b.Nombre)
            .ToListAsync();
    }

    public async Task<Bodega?> GetById(int id)
    {
        return await _context.Bodegas
            .Include(b => b.Stocks)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Bodega?> GetByNombre(string nombre)
    {
        return await _context.Bodegas
            .FirstOrDefaultAsync(b => b.Nombre.ToLower() == nombre.ToLower());
    }

    public async Task<(IEnumerable<Bodega>, int)> GetWithFilters(FiltroBodegaDto filtro)
    {
        var query = _context.Bodegas.AsQueryable();

        // Filtro por estado activo/inactivo
        if (filtro.SoloActivas.HasValue)
        {
            query = query.Where(b => b.Activa == filtro.SoloActivas.Value);
        }

        // Filtro de búsqueda por nombre, responsable o dirección
        if (!string.IsNullOrWhiteSpace(filtro.TerminoBusqueda))
        {
            var termino = filtro.TerminoBusqueda.ToLower();
            query = query.Where(b =>
                b.Nombre.ToLower().Contains(termino) ||
                (b.Responsable != null && b.Responsable.ToLower().Contains(termino)) ||
                (b.Direccion != null && b.Direccion.ToLower().Contains(termino))
            );
        }

        // Contar total antes de paginar
        var total = await query.CountAsync();

        // Ordenamiento
        query = filtro.OrdenarPor?.ToLower() switch
        {
            "fechacreacion" => filtro.Descendente
                ? query.OrderByDescending(b => b.FechaCreacion)
                : query.OrderBy(b => b.FechaCreacion),
            _ => filtro.Descendente
                ? query.OrderByDescending(b => b.Nombre)
                : query.OrderBy(b => b.Nombre)
        };

        // Paginación
        if (filtro.Pagina > 0 && filtro.ElementosPorPagina > 0)
        {
            query = query
                .Skip((filtro.Pagina - 1) * filtro.ElementosPorPagina)
                .Take(filtro.ElementosPorPagina);
        }

        var bodegas = await query
            .Include(b => b.Stocks)
            .ToListAsync();

        return (bodegas, total);
    }

    public async Task<Bodega> Create(Bodega bodega)
    {
        bodega.FechaCreacion = DateTime.UtcNow;
        bodega.FechaActualizacion = DateTime.UtcNow;

        _context.Bodegas.Add(bodega);
        await _context.SaveChangesAsync();

        return bodega;
    }

    public async Task<Bodega> Update(Bodega bodega)
    {
        bodega.FechaActualizacion = DateTime.UtcNow;

        _context.Bodegas.Update(bodega);
        await _context.SaveChangesAsync();

        return bodega;
    }

    public async Task<bool> Delete(int id)
    {
        var bodega = await GetById(id);
        if (bodega == null)
            return false;

        // Soft delete
        bodega.Activa = false;
        bodega.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExisteNombre(string nombre, int? idExcluir = null)
    {
        var query = _context.Bodegas.Where(b => b.Nombre.ToLower() == nombre.ToLower());

        if (idExcluir.HasValue)
        {
            query = query.Where(b => b.Id != idExcluir.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<int> GetTotalProductos(int idBodega)
    {
        return await _context.Stocks
            .Where(s => s.IdBodega == idBodega)
            .CountAsync();
    }
}