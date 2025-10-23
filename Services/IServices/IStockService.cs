using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.IServices;

public interface IStockService
{
    Task<IEnumerable<StockDto>> GetAll();
    Task<StockDto?> GetById(int id);
    Task<StockDto?> GetByProductoYBodega(int idProducto, int idBodega);
    Task<(IEnumerable<StockDto>, int)> GetWithFilters(FiltroStockDto filtro);
    Task<IEnumerable<StockDto>> GetStockBajo();
    Task<IEnumerable<StockDto>> GetByBodega(int idBodega);
    Task<IEnumerable<StockDto>> GetByProducto(int idProducto);
}