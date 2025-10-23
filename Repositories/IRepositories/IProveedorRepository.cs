using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Models;

namespace ProyectoGraduación.Repositories.IRepositories;

public interface IProveedorRepository
    

{
    Task<IEnumerable<Proveedor>> GetAll();

    Task<(IEnumerable<Proveedor> proveedores, int total)> GetWithFilters(
        string? terminoBusqueda,
        bool? activo,
        int numeroPagina,
        int tamanoPagina);


    Task<Proveedor?> GetById(int id);


    Task<IEnumerable<Proveedor>> SearchByNombre(string termino);


    Task Add(Proveedor proveedor);


    Task Update(Proveedor proveedor);

    Task Delete(int id);


    Task<bool> ExisteNombre(string nombre, int? excludeId = null);
}