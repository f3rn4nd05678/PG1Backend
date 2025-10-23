using ProyectoGraduación.Models;

namespace ProyectoGraduación.Repositories.IRepositories;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> GetAll();
    Task<Cliente?> GetById(int id);
    Task<Cliente?> GetByNit(string nit);
    Task<Cliente?> GetByCodigo(string codigo);
    Task<Cliente> Create(Cliente cliente);
    Task<bool> Update(Cliente cliente);
    Task<bool> Delete(int id);
    Task<IEnumerable<Cliente>> Search(string? nombre, string? nit, string? codigo);
    Task<bool> ExisteNit(string nit, int? idExcluir = null);
    Task<bool> ExisteCodigo(string codigo, int? idExcluir = null);
}