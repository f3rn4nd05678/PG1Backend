using ProyectoGraduación.DTOs;
using ProyectoGraduación.Models;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> GetAll();
    Task<(IEnumerable<Cliente> clientes, int total)> GetWithFilters(FiltroClienteDto filtro);
    Task<Cliente?> GetById(int id);
    Task<IEnumerable<Cliente>> GetMultipleByCodigoNitOrNombre(string terminoBusqueda);
    Task<Cliente?> GetByNit(string nit);
    Task<Cliente?> GetByCodigo(string codigo);
    Task<Cliente> Add(Cliente cliente);
    Task<bool> Update(Cliente cliente);
    Task<bool> Delete(int id);
    Task Disable(int id, int usuarioId);
    Task<bool> ExisteCodigo(string codigo);
    Task<bool> ExisteNit(string nit);
    Task<bool> ExisteNit(string nit, int? idExcluir);  // <- AGREGAR sobrecarga
    Task<string> GenerateNextCodigo();
    Task<Cliente?> GetByCodigoOrNit(string codigoOrNit);
}