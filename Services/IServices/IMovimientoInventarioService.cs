using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.IServices;

public interface IMovimientoInventarioService
{
    Task<IEnumerable<MovimientoInventarioDto>> GetAll();
    Task<MovimientoInventarioDto?> GetById(int id);
    Task<(IEnumerable<MovimientoInventarioDto>, int)> GetWithFilters(FiltroMovimientoInventarioDto filtro);
    Task<MovimientoInventarioDto> RegistrarEntrada(CrearEntradaInventarioDto entrada, int usuarioId);
    Task<MovimientoInventarioDto> RegistrarSalida(CrearEntradaInventarioDto salida, int usuarioId);
    Task<IEnumerable<MovimientoInventarioDto>> GetByProducto(int idProducto);
    Task<IEnumerable<MovimientoInventarioDto>> GetByBodega(int idBodega);
}