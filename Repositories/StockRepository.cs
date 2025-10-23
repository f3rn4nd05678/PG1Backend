using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Data;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.Models;

namespace ProyectoGraduación.Repositories;

public class StockRepository : IStockRepository
{
    private readonly AppDbContext _context;

    public StockRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Stock>> GetAll()
    {
        return await _context.Stocks
            .Include(s => s.Producto)
                .ThenInclude(p => p.Categoria)
            .Include(s => s.Bodega)
            .OrderBy(s => s.Producto.Nombre)
            .ToListAsync();
    }

    public async Task<Stock?> GetById(int id)
    {
        return await _context.Stocks
            .Include(s => s.Producto)
                .ThenInclude(p => p.Categoria)
            .Include(s => s.Bodega)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Stock?> GetByProductoYBodega(int idProducto, int idBodega)
    {
        return await _context.Stocks
            .Include(s => s.Producto)
            .Include(s => s.Bodega)
            .FirstOrDefaultAsync(s => s.IdProducto == idProducto && s.IdBodega == idBodega);
    }

    public async Task<(IEnumerable<Stock>, int)> GetWithFilters(FiltroStockDto filtro)
    {
        var query = _context.Stocks
            .Include(s => s.Producto)
                .ThenInclude(p => p.Categoria)
            .Include(s => s.Bodega)
            .AsQueryable();

        // Filtro por búsqueda
        if (!string.IsNullOrWhiteSpace(filtro.TerminoBusqueda))
        {
            var termino = filtro.TerminoBusqueda.ToLower();
            query = query.Where(s =>
                s.Producto.Nombre.ToLower().Contains(termino) ||
                s.Producto.Codigo.ToLower().Contains(termino) ||
                s.Bodega.Nombre.ToLower().Contains(termino)
            );
        }

        // Filtro por bodega
        if (filtro.IdBodega.HasValue)
        {
            query = query.Where(s => s.IdBodega == filtro.IdBodega.Value);
        }

        // Filtro por categoría
        if (filtro.IdCategoria.HasValue)
        {
            query = query.Where(s => s.Producto.CategoriaId == filtro.IdCategoria.Value);
        }

        // Filtro por nivel de alerta
        if (!string.IsNullOrWhiteSpace(filtro.NivelAlerta))
        {
            switch (filtro.NivelAlerta.ToUpper())
            {
                case "SIN_STOCK":
                    query = query.Where(s => s.CantidadDisponible <= 0);
                    break;
                case "CRITICO":
                    query = query.Where(s => s.CantidadDisponible > 0 && s.CantidadDisponible <= s.CantidadMinima);
                    break;
                case "BAJO":
                    query = query.Where(s => s.CantidadDisponible > s.CantidadMinima &&
                                           s.CantidadDisponible <= s.CantidadMinima * 1.5m);
                    break;
                case "NORMAL":
                    query = query.Where(s => s.CantidadDisponible > s.CantidadMinima * 1.5m);
                    break;
            }
        }

        // Contar total
        var total = await query.CountAsync();

        // Ordenamiento
        query = filtro.OrdenarPor?.ToLower() switch
        {
            "bodega" => filtro.Descendente
                ? query.OrderByDescending(s => s.Bodega.Nombre)
                : query.OrderBy(s => s.Bodega.Nombre),
            "cantidad" => filtro.Descendente
                ? query.OrderByDescending(s => s.CantidadDisponible)
                : query.OrderBy(s => s.CantidadDisponible),
            "alerta" => query.OrderBy(s =>
                s.CantidadDisponible <= 0 ? 1 :
                s.CantidadDisponible <= s.CantidadMinima ? 2 :
                s.CantidadDisponible <= s.CantidadMinima * 1.5m ? 3 : 4),
            _ => filtro.Descendente
                ? query.OrderByDescending(s => s.Producto.Nombre)
                : query.OrderBy(s => s.Producto.Nombre)
        };

        // Paginación
        if (filtro.Pagina > 0 && filtro.ElementosPorPagina > 0)
        {
            query = query
                .Skip((filtro.Pagina - 1) * filtro.ElementosPorPagina)
                .Take(filtro.ElementosPorPagina);
        }

        var stocks = await query.ToListAsync();

        return (stocks, total);
    }

    public async Task<Stock> CreateOrUpdate(Stock stock)
    {
        var stockExistente = await GetByProductoYBodega(stock.IdProducto, stock.IdBodega);

        if (stockExistente != null)
        {
            stockExistente.CantidadActual = stock.CantidadActual;
            stockExistente.CantidadMinima = stock.CantidadMinima;
            stockExistente.FechaActualizacion = DateTime.UtcNow;

            _context.Stocks.Update(stockExistente);
        }
        else
        {
            stock.FechaActualizacion = DateTime.UtcNow;
            _context.Stocks.Add(stock);
        }

        await _context.SaveChangesAsync();

        return stockExistente ?? stock;
    }

    public async Task<IEnumerable<Stock>> GetStockBajo()
    {
        return await _context.Stocks
            .Include(s => s.Producto)
            .Include(s => s.Bodega)
            .Where(s => s.CantidadDisponible <= s.CantidadMinima)
            .OrderBy(s => s.CantidadDisponible)
            .ToListAsync();
    }

    public async Task<IEnumerable<Stock>> GetByBodega(int idBodega)
    {
        return await _context.Stocks
            .Include(s => s.Producto)
                .ThenInclude(p => p.Categoria)
            .Where(s => s.IdBodega == idBodega)
            .OrderBy(s => s.Producto.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Stock>> GetByProducto(int idProducto)
    {
        return await _context.Stocks
            .Include(s => s.Bodega)
            .Where(s => s.IdProducto == idProducto)
            .OrderBy(s => s.Bodega.Nombre)
            .ToListAsync();
    }

    public async Task<bool> ActualizarStock(int idProducto, int idBodega, decimal cantidadCambio, string tipoMovimiento)
    {
        var stock = await GetByProductoYBodega(idProducto, idBodega);

        if (stock == null)
        {
            // Si no existe, crear uno nuevo
            stock = new Stock
            {
                IdProducto = idProducto,
                IdBodega = idBodega,
                CantidadActual = 0,
                CantidadMinima = 0
            };
            _context.Stocks.Add(stock);
        }

        // Actualizar según tipo de movimiento
        switch (tipoMovimiento.ToUpper())
        {
            case "ENTRADA":
            case "AJUSTE":
                stock.CantidadActual += cantidadCambio;
                stock.UltimaEntrada = DateTime.UtcNow;
                break;
            case "SALIDA":
            case "TRANSFERENCIA":
                if (stock.CantidadActual < cantidadCambio)
                {
                    throw new InvalidOperationException("Stock insuficiente");
                }
                stock.CantidadActual -= cantidadCambio;
                stock.UltimaSalida = DateTime.UtcNow;
                break;
        }

        stock.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}