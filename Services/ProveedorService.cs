using ProyectoGraduación.Models;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Services.IServices;
using ProyectoGraduación.Repositories.IRepositories;

namespace ProyectoGraduación.Services;

public class ProveedorService : IProveedorService
{
    private readonly IProveedorRepository _proveedorRepository;

    public ProveedorService(IProveedorRepository proveedorRepository)
    {
        _proveedorRepository = proveedorRepository;
    }

    public async Task<IEnumerable<ProveedorDto>> GetAll()
    {
        var proveedores = await _proveedorRepository.GetAll();
        return proveedores.Select(MapToDto);
    }

    public async Task<(IEnumerable<ProveedorDto> proveedores, int total)> GetWithFilters(FiltroProveedorDto filtro)
    {
        var resultado = await _proveedorRepository.GetWithFilters(
            filtro.TerminoBusqueda,
            filtro.Activo,
            filtro.NumeroPagina,
            filtro.TamanoPagina);

        var proveedoresDto = resultado.Item1.Select(MapToDto).ToList();
        return (proveedoresDto, resultado.Item2);
    }

    public async Task<ProveedorDto?> GetById(int id)
    {
        var proveedor = await _proveedorRepository.GetById(id);
        return proveedor == null ? null : MapToDto(proveedor);
    }

    public async Task<IEnumerable<ProveedorDto>> SearchByNombre(string termino)
    {
        var proveedores = await _proveedorRepository.SearchByNombre(termino);
        return proveedores.Select(MapToDto);
    }

    public async Task<ProveedorDto> CreateProveedor(CrearProveedorDto crearProveedorDto)
    {
        // Validar que el nombre no exista
        if (await _proveedorRepository.ExisteNombre(crearProveedorDto.Nombre))
        {
            throw new InvalidOperationException($"Ya existe un proveedor con el nombre '{crearProveedorDto.Nombre}'");
        }

        var proveedor = new Proveedor
        {
            Nombre = crearProveedorDto.Nombre,
            Contacto = crearProveedorDto.Contacto,
            Nit = crearProveedorDto.Nit,
            Direccion = crearProveedorDto.Direccion,
            Telefono = crearProveedorDto.Telefono,
            Email = crearProveedorDto.Email,
            Activo = true
        };

        await _proveedorRepository.Add(proveedor);

        // Obtener el proveedor creado con sus relaciones
        var proveedorCreado = await _proveedorRepository.GetById(proveedor.Id);
        return MapToDto(proveedorCreado!);
    }

    public async Task<ProveedorDto> UpdateProveedor(int id, ActualizarProveedorDto actualizarProveedorDto)
    {
        var proveedor = await _proveedorRepository.GetById(id);
        if (proveedor == null)
        {
            throw new KeyNotFoundException($"No se encontró el proveedor con ID {id}");
        }

        // Validar que el nombre no esté en uso por otro proveedor
        if (proveedor.Nombre != actualizarProveedorDto.Nombre &&
            await _proveedorRepository.ExisteNombre(actualizarProveedorDto.Nombre, id))
        {
            throw new InvalidOperationException($"Ya existe un proveedor con el nombre '{actualizarProveedorDto.Nombre}'");
        }

        // Actualizar campos
        proveedor.Nombre = actualizarProveedorDto.Nombre;
        proveedor.Contacto = actualizarProveedorDto.Contacto;
        proveedor.Nit = actualizarProveedorDto.Nit;
        proveedor.Direccion = actualizarProveedorDto.Direccion;
        proveedor.Telefono = actualizarProveedorDto.Telefono;
        proveedor.Email = actualizarProveedorDto.Email;
        proveedor.Activo = actualizarProveedorDto.Activo;

        await _proveedorRepository.Update(proveedor);

        // Obtener el proveedor actualizado con sus relaciones
        var proveedorActualizado = await _proveedorRepository.GetById(id);
        return MapToDto(proveedorActualizado!);
    }

    public async Task DeleteProveedor(int id)
    {
        var proveedor = await _proveedorRepository.GetById(id);
        if (proveedor == null)
        {
            throw new KeyNotFoundException($"No se encontró el proveedor con ID {id}");
        }

        // Verificar si tiene productos asociados
        if (proveedor.Productos.Any())
        {
            throw new InvalidOperationException($"No se puede eliminar el proveedor porque tiene {proveedor.Productos.Count} producto(s) asociado(s)");
        }

        await _proveedorRepository.Delete(id);
    }

    public async Task<bool> ExisteNombre(string nombre, int? excludeId = null)
    {
        return await _proveedorRepository.ExisteNombre(nombre, excludeId);
    }

    private ProveedorDto MapToDto(Proveedor proveedor)
    {
        return new ProveedorDto
        {
            Id = proveedor.Id,
            Nombre = proveedor.Nombre,
            Contacto = proveedor.Contacto,
            Nit = proveedor.Nit,
            Direccion = proveedor.Direccion,
            Telefono = proveedor.Telefono,
            Email = proveedor.Email,
            Activo = proveedor.Activo,
            CantidadProductos = proveedor.Productos?.Count ?? 0
        };
    }
}
