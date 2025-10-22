using ProyectoGraduación.Models;
using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.IServices;

public interface IClienteService
{
    Task<IEnumerable<ClienteDto>> GetAll();
    Task<(IEnumerable<ClienteDto> clientes, int total)> GetWithFilters(FiltroClienteDto filtro);
    Task<ClienteDto?> GetById(int id);
    Task<ClienteDto?> GetByCodigoOrNit(string codigoOrNit);
    Task<IEnumerable<ClienteDto>> GetMultipleByCodigoNitOrNombre(string terminoBusqueda);
    Task<ClienteDto> CreateCliente(CrearClienteDto crearClienteDto, int usuarioId);
    Task<ClienteDto> UpdateCliente(int id, ActualizarClienteDto actualizarClienteDto, int usuarioId);
    Task DisableCliente(int id, int usuarioId);
    Task DeleteCliente(int id, int usuarioId);
    Task<bool> ExisteCodigo(string codigo);
    Task<bool> ExisteNit(string nit);
}