using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Models;
using ProyectoGraduación.Data;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.DTOs;
using Microsoft.Extensions.Logging;

namespace ProyectoGraduación.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ClienteRepository> _logger;

    public ClienteRepository(AppDbContext context, ILogger<ClienteRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Cliente>> GetAll()
    {
        return await _context.Clientes
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Cliente> clientes, int total)> GetWithFilters(FiltroClienteDto filtro)
    {
        var query = _context.Clientes.AsQueryable();


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


        var total = await query.CountAsync();

        var clientes = await query
            .OrderBy(c => c.Nombre)
            .Skip((filtro.Pagina - 1) * filtro.ElementosPorPagina)
            .Take(filtro.ElementosPorPagina)
            .ToListAsync();

        return (clientes, total);
    }

    public async Task<Cliente?> GetById(int id)
    {
        try
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cliente por ID: {Id}", id);
            throw;
        }
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
            .Take(10) 
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
        try
        {
            cliente.FechaCreacion = DateTime.UtcNow;
            cliente.FechaActualizacion = DateTime.UtcNow;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cliente creado con ID: {Id}", cliente.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear cliente: {Message}", ex.Message);
            throw;
        }
    }

    public async Task Update(Cliente cliente)
    {
        try
        {
            
            cliente.FechaActualizacion = DateTime.UtcNow;

           
            var entry = _context.Entry(cliente);

            if (entry.State == EntityState.Detached)
            {
                _context.Clientes.Attach(cliente);
                entry.State = EntityState.Modified;
            }

            _logger.LogInformation("Intentando actualizar cliente ID: {Id}", cliente.Id);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Cliente actualizado exitosamente ID: {Id}", cliente.Id);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Error de base de datos al actualizar cliente ID: {Id}. Detalles: {InnerException}",
                cliente.Id, dbEx.InnerException?.Message);
            throw new Exception($"Error en la base de datos al actualizar el cliente: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error general al actualizar cliente ID: {Id}", cliente.Id);
            throw new Exception($"Error al actualizar el cliente: {ex.Message}");
        }
    }

    public async Task Delete(int id)
    {
        try
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
               
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cliente eliminado completamente ID: {Id}", id);
            }
            else
            {
                _logger.LogWarning("Intento de eliminar cliente que no existe ID: {Id}", id);
                throw new Exception("Cliente no encontrado");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar cliente ID: {Id}", id);
            throw;
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
        try
        {
            var ultimoCodigo = await _context.Clientes
                .Where(c => c.Codigo.StartsWith("C"))
                .OrderByDescending(c => c.Id)
                .Select(c => c.Codigo)
                .FirstOrDefaultAsync();

            if (ultimoCodigo == null)
            {
                return "C-000001";
            }

           
            var numeroStr = ultimoCodigo.Substring(2); 
            if (int.TryParse(numeroStr, out int numero))
            {
                return $"C-{(numero + 1):D6}";
            }

            return "C-000001";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar siguiente código");
            throw;
        }
    }

    public async Task<Cliente?> GetByCodigoOrNit(string codigoOrNit)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Codigo == codigoOrNit || c.Nit == codigoOrNit);
    }
}