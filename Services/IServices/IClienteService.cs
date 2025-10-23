using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.Services.IServices;

public interface IClienteService
{
    // Métodos principales
    Task<IEnumerable<ClienteDto>> ObtenerTodosAsync();
    Task<ClienteDto?> ObtenerPorIdAsync(int id);
    Task<ClienteDto?> ObtenerPorNitAsync(string nit);
    Task<ClienteDto> CrearAsync(CrearClienteDto dto);
    Task<bool> ActualizarAsync(int id, ActualizarClienteDto dto);
    Task<bool> EliminarAsync(int id);
    Task<IEnumerable<ClienteDto>> BuscarAsync(string? nombre, string? nit, string? codigo);

    // Métodos de compatibilidad (alias)
    Task<IEnumerable<ClienteDto>> GetAll();
    Task<(IEnumerable<ClienteDto>, int)> GetWithFilters(FiltroClienteDto filtro);
    Task<ClienteDto?> GetById(int id);
    Task<ClienteDto?> GetByCodigoOrNit(string codigoOrNit);
    Task<IEnumerable<ClienteDto>> GetMultipleByCodigoNitOrNombre(string termino);
    Task<ClienteDto> CreateCliente(CrearClienteDto dto, int usuarioId);
    Task<bool> UpdateCliente(int id, ActualizarClienteDto dto, int usuarioId);
    Task<bool> DisableCliente(int id, int usuarioId);
    Task<bool> DeleteCliente(int id, int usuarioId);
    Task<bool> ExisteCodigo(string codigo);
    Task<bool> ExisteNit(string nit);
    Task<bool> ExisteNit(string nit, int? idExcluir);
}