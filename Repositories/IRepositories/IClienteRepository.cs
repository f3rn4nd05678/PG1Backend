using ProyectoGraduación.Models;
using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.IRepositories;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> GetAll();
    Task<(IEnumerable<Cliente> clientes, int total)> GetWithFilters(FiltroClienteDto filtro);
    Task<Cliente?> GetById(int id);
    Task<Cliente?> GetByCodigoOrNit(string codigoOrNit);
    Task<IEnumerable<Cliente>> GetMultipleByCodigoNitOrNombre(string terminoBusqueda);
    Task<Cliente?> GetByNit(string nit);
    Task<Cliente?> GetByCodigo(string codigo);
    Task Add(Cliente cliente);
    Task Update(Cliente cliente);
    Task Delete(int id);
    Task<bool> ExisteCodigo(string codigo);
    Task<bool> ExisteNit(string nit);
    Task<string> GenerateNextCodigo();
}