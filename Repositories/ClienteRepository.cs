using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Models;
using ProyectoGraduación.Data;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;


    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cliente>> GetAll()
    {
        return await _context.Clientes
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Cliente> clientes, int total)> GetWithFilters(FiltroClienteDto filtro)
    {
        var query = _context.Clientes.AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrEmpty(filtro.Nombre))
        {
            query = query.Where(c => c.Nombre.ToLower().Contains(filtro.Nombre.ToLower()) ||
                                    (c.NombreExtranjero != null && c.NombreExtranjero.ToLower().Contains(filtro.Nombre.ToLower())));
        }

        if (!string.IsNullOrEmpty(filtro.Nit))
        {
            query = query.Where(c => c.Nit != null && c.Nit.Contains(filtro.Nit));
        }

        if (!string.IsNullOrEmpty(filtro.Codigo))
        {
            query = query.Where(c => c.Codigo.Contains(filtro.Codigo));
        }

        if (!string.IsNullOrEmpty(filtro.Grupo))
        {
            query = query.Where(c => c.Grupo != null && c.Grupo.ToLower().Contains(filtro.Grupo.ToLower()));
        }

        if (filtro.Activo.HasValue)
        {
            query = query.Where(c => c.Activo == filtro.Activo.Value);
        }

        // Contar total antes de aplicar paginación
        var total = await query.CountAsync();

        // Aplicar paginación
        var clientes = await query
            .OrderBy(c => c.Nombre)
            .Skip((filtro.Pagina - 1) * filtro.ElementosPorPagina)
            .Take(filtro.ElementosPorPagina)
            .ToListAsync();

        return (clientes, total);
    }

    public async Task<Cliente?> GetById(int id)
    {
        // Eliminamos los Include ya que parecen estar causando problemas
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == id);
    }


    public async Task<IEnumerable<Cliente>> GetMultipleByCodigoNitOrNombre(string terminoBusqueda)
    {
        return await _context.Clientes
            .Where(c => c.Activo && (
                c.Codigo == terminoBusqueda ||
                c.Nit == terminoBusqueda ||
                c.Nombre.ToLower().Contains(terminoBusqueda.ToLower()) ||
                (c.NombreExtranjero != null && c.NombreExtranjero.ToLower().Contains(terminoBusqueda.ToLower()))
            ))
            .OrderBy(c => c.Nombre)
            .Take(10) // Limitar a 10 resultados
            .ToListAsync();
    }


    public async Task<Cliente?> GetByNit(string nit)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Nit == nit);
    }

    public async Task<Cliente?> GetByCodigo(string codigo)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Codigo == codigo);
    }

    public async Task Add(Cliente cliente)
    {
        cliente.FechaCreacion = DateTime.UtcNow;
        cliente.FechaActualizacion = DateTime.UtcNow;
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Cliente cliente)
    {
        cliente.FechaActualizacion = DateTime.UtcNow;
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente != null)
        {
            // Soft delete - marcar como inactivo en lugar de eliminar
            cliente.Activo = false;
            cliente.FechaActualizacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExisteCodigo(string codigo)
    {
        return await _context.Clientes.AnyAsync(c => c.Codigo == codigo);
    }

    public async Task<bool> ExisteNit(string nit)
    {
        return await _context.Clientes.AnyAsync(c => c.Nit == nit);
    }

    public async Task<string> GenerateNextCodigo()
    {
        // Generar código automático basado en el siguiente número disponible
        var ultimoCodigo = await _context.Clientes
            .Where(c => c.Codigo.StartsWith("C"))
            .OrderByDescending(c => c.Id)
            .Select(c => c.Codigo)
            .FirstOrDefaultAsync();

        if (ultimoCodigo == null)
        {
            return "C-000001";
        }

        // Extraer el número del código y incrementar
        var numeroStr = ultimoCodigo.Substring(2); // Quitar "C-"
        if (int.TryParse(numeroStr, out int numero))
        {
            return $"C-{(numero + 1):D6}";
        }

        return "C-000001";
    }

    public async Task<Cliente?> GetByCodigoOrNit(string codigoOrNit)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Codigo == codigoOrNit || c.Nit == codigoOrNit);
    }
}