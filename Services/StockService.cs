using ProyectoGraduación.DTOs;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Models;

namespace ProyectoGraduación.Services;

public class StockService : IStockService
{
    private readonly IStockRepository _stockRepository;
    private readonly ILogger<StockService> _logger;

    public StockService(
        IStockRepository stockRepository,
        ILogger<StockService> logger)
    {
        _stockRepository = stockRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<StockDto>> GetAll()
    {
        var stocks = await _stockRepository.GetAll();
        return stocks.Select(MapToDto);
    }

    public async Task<StockDto?> GetById(int id)
    {
        var stock = await _stockRepository.GetById(id);
        return stock != null ? MapToDto(stock) : null;
    }

    public async Task<StockDto?> GetByProductoYBodega(int idProducto, int idBodega)
    {
        var stock = await _stockRepository.GetByProductoYBodega(idProducto, idBodega);
        return stock != null ? MapToDto(stock) : null;
    }

    public async Task<(IEnumerable<StockDto>, int)> GetWithFilters(FiltroStockDto filtro)
    {
        var (stocks, total) = await _stockRepository.GetWithFilters(filtro);
        var stocksDto = stocks.Select(MapToDto);
        return (stocksDto, total);
    }

    public async Task<IEnumerable<StockDto>> GetStockBajo()
    {
        var stocks = await _stockRepository.GetStockBajo();
        return stocks.Select(MapToDto);
    }

    public async Task<IEnumerable<StockDto>> GetByBodega(int idBodega)
    {
        var stocks = await _stockRepository.GetByBodega(idBodega);
        return stocks.Select(MapToDto);
    }

    public async Task<IEnumerable<StockDto>> GetByProducto(int idProducto)
    {
        var stocks = await _stockRepository.GetByProducto(idProducto);
        return stocks.Select(MapToDto);
    }

    private static StockDto MapToDto(Stock stock)
    {
        // Calcular nivel de alerta
        string nivelAlerta = "NORMAL";
        if (stock.CantidadDisponible <= 0)
        {
            nivelAlerta = "SIN_STOCK";
        }
        else if (stock.CantidadDisponible <= stock.CantidadMinima)
        {
            nivelAlerta = "CRITICO";
        }
        else if (stock.CantidadDisponible <= stock.CantidadMinima * 1.5m)
        {
            nivelAlerta = "BAJO";
        }

        return new StockDto
        {
            Id = stock.Id,
            IdProducto = stock.IdProducto,
            CodigoProducto = stock.Producto?.Codigo ?? "",
            NombreProducto = stock.Producto?.Nombre ?? "",
            CategoriaProducto = stock.Producto?.Categoria?.Nombre ?? "",
            IdBodega = stock.IdBodega,
            NombreBodega = stock.Bodega?.Nombre ?? "",
            CantidadActual = stock.CantidadActual,
            CantidadMinima = stock.CantidadMinima,
            CantidadReservada = stock.CantidadReservada,
            CantidadDisponible = stock.CantidadDisponible,
            NivelAlerta = nivelAlerta,
            UltimaEntrada = stock.UltimaEntrada,
            UltimaSalida = stock.UltimaSalida
        };
    }
}