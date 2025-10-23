using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.IServices;

public interface IBodegaService
{
    Task<IEnumerable<BodegaDto>> GetAll();
    Task<BodegaDto?> GetById(int id);
    Task<(IEnumerable<BodegaDto>, int)> GetWithFilters(FiltroBodegaDto filtro);
    Task<BodegaDto> Create(CrearBodegaDto dto, int usuarioId);
    Task<BodegaDto> Update(int id, ActualizarBodegaDto dto, int usuarioId);
    Task<bool> Delete(int id, int usuarioId);
    Task<bool> CambiarEstado(int id, bool activa, int usuarioId);
    Task<IEnumerable<StockAlertaDto>> GetAlertasStock(int idBodega);
    Task<bool> ExisteNombre(string nombre, int? idExcluir = null);
}