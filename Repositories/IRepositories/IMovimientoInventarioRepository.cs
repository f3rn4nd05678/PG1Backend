using ProyectoGraduación.DTOs;
using ProyectoGraduación.Models;

namespace ProyectoGraduación.IRepositories;

public interface IMovimientoInventarioRepository
{
    Task<IEnumerable<MovimientoInventario>> GetAll();
    Task<MovimientoInventario?> GetById(int id);
    Task<(IEnumerable<MovimientoInventario>, int)> GetWithFilters(FiltroMovimientoInventarioDto filtro);
    Task<MovimientoInventario> Create(MovimientoInventario movimiento);
    Task<IEnumerable<MovimientoInventario>> GetByProducto(int idProducto);
    Task<IEnumerable<MovimientoInventario>> GetByBodega(int idBodega);
    Task<IEnumerable<MovimientoInventario>> GetByFechaRango(DateTime fechaDesde, DateTime fechaHasta);
}