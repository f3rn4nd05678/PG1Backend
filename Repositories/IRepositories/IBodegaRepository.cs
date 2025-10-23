using ProyectoGraduación.Models;
using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.IRepositories;

public interface IBodegaRepository
{
    Task<IEnumerable<Bodega>> GetAll();
    Task<Bodega?> GetById(int id);
    Task<Bodega?> GetByNombre(string nombre);
    Task<(IEnumerable<Bodega>, int)> GetWithFilters(FiltroBodegaDto filtro);
    Task<Bodega> Create(Bodega bodega);
    Task<Bodega> Update(Bodega bodega);
    Task<bool> Delete(int id); // Soft delete
    Task<bool> ExisteNombre(string nombre, int? idExcluir = null);
    Task<int> GetTotalProductos(int idBodega);
}