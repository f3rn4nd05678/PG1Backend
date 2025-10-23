using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.Services.IServices;

public interface IProveedorService
{
    Task<IEnumerable<ProveedorDto>> ObtenerTodosAsync();
    Task<ProveedorDto?> ObtenerPorIdAsync(int id);
    Task<ProveedorDto> CrearAsync(CrearProveedorDto dto);
    Task<bool> ActualizarAsync(int id, ActualizarProveedorDto dto);
    Task<bool> EliminarAsync(int id);
    Task<IEnumerable<ProveedorDto>> BuscarAsync(string? termino);

    // Métodos de compatibilidad
    Task<IEnumerable<ProveedorDto>> GetAll();
    Task<(IEnumerable<ProveedorDto>, int)> GetWithFilters(FiltroProveedorDto filtro);
    Task<ProveedorDto?> GetById(int id);
    Task<ProveedorDto> Create(CrearProveedorDto dto);
    Task<bool> Update(int id, ActualizarProveedorDto dto);
    Task<bool> Delete(int id);
}