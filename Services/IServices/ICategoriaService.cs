using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.Services.IServices;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaDto>> ObtenerTodosAsync();
    Task<IEnumerable<CategoriaDto>> ObtenerActivosAsync();
    Task<CategoriaDto?> ObtenerPorIdAsync(int id);
    Task<CategoriaDto> CrearAsync(CrearCategoriaDto dto);
    Task<CategoriaDto> ActualizarAsync(int id, ActualizarCategoriaDto dto);
    Task EliminarAsync(int id);
    Task<bool> ExisteCodigoPrefijo(string codigoPrefijo, int? idExcluir = null);
    Task<bool> ExisteNombre(string nombre, int? idExcluir = null);
}