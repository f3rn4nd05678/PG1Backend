using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.IServices;

public interface IProveedorService
{
    Task<IEnumerable<ProveedorDto>> GetAll();
    Task<(IEnumerable<ProveedorDto> proveedores, int total)> GetWithFilters(FiltroProveedorDto filtro);
    Task<ProveedorDto?> GetById(int id);
    Task<IEnumerable<ProveedorDto>> SearchByNombre(string termino);
    Task<ProveedorDto> CreateProveedor(CrearProveedorDto crearProveedorDto);
    Task<ProveedorDto> UpdateProveedor(int id, ActualizarProveedorDto actualizarProveedorDto);
    Task DeleteProveedor(int id);
    Task<bool> ExisteNombre(string nombre, int? excludeId = null);
}