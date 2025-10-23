using ProyectoGraduación.DTOs;
using ProyectoGraduación.Models;

namespace ProyectoGraduación.IRepositories;

public interface IStockRepository
{
    Task<IEnumerable<Stock>> GetAll();
    Task<Stock?> GetById(int id);
    Task<Stock?> GetByProductoYBodega(int idProducto, int idBodega);
    Task<(IEnumerable<Stock>, int)> GetWithFilters(FiltroStockDto filtro);
    Task<Stock> CreateOrUpdate(Stock stock);
    Task<IEnumerable<Stock>> GetStockBajo(); // Stock con alertas
    Task<IEnumerable<Stock>> GetByBodega(int idBodega);
    Task<IEnumerable<Stock>> GetByProducto(int idProducto);
    Task<bool> ActualizarStock(int idProducto, int idBodega, decimal cantidadCambio, string tipoMovimiento);
}