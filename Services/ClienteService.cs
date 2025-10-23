using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Data;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.Models;
using ProyectoGraduación.Repositories.IRepositories;
using ProyectoGraduación.Services.IServices;

namespace ProyectoGraduación.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository repo)
    {
        _clienteRepository = repo;
    }

    // Métodos principales
    public async Task<IEnumerable<ClienteDto>> ObtenerTodosAsync()
    {
        var clientes = await _clienteRepository.GetAll();
        return clientes.Select(c => MapToDto(c)).ToList();
    }

    public async Task<ClienteDto?> ObtenerPorIdAsync(int id)
    {
        var cliente = await _clienteRepository.GetById(id);
        return cliente != null ? MapToDto(cliente) : null;
    }

    public async Task<ClienteDto?> ObtenerPorNitAsync(string nit)
    {
        var cliente = await _clienteRepository.GetByNit(nit);
        return cliente != null ? MapToDto(cliente) : null;
    }

    public async Task<ClienteDto> CrearAsync(CrearClienteDto dto)
    {
        // Validar NIT único si se proporciona
        if (!string.IsNullOrEmpty(dto.Nit))
        {
            var existeNit = await _clienteRepository.ExisteNit(dto.Nit);  // <- Usar repositorio
            if (existeNit)
                throw new InvalidOperationException("Ya existe un cliente con ese NIT");
        }

        var cliente = new Cliente
        {
            // Codigo se genera automáticamente por trigger
            TipoCliente = dto.TipoCliente,
            Nombre = dto.Nombre,
            NombreExtranjero = dto.NombreExtranjero,
            Grupo = dto.Grupo,
            Moneda = dto.Moneda,
            Nit = dto.Nit,
            Direccion = dto.Direccion,
            Telefono1 = dto.Telefono1,
            Telefono2 = dto.Telefono2,
            TelefonoMovil = dto.TelefonoMovil,
            Fax = dto.Fax,
            CorreoElectronico = dto.CorreoElectronico,
            SitioWeb = dto.SitioWeb,
            Posicion = dto.Posicion,
            Titulo = dto.Titulo,
            SegundoNombre = dto.SegundoNombre,
            Apellido = dto.Apellido,
            LimiteCredito = dto.LimiteCredito,
            DiasCredito = dto.DiasCredito,
            DescuentoPorcentaje = dto.DescuentoPorcentaje,
            BloquearMarketing = false,
            Observaciones1 = dto.Observaciones1,
            Observaciones2 = dto.Observaciones2,
            ClaveAcceso = dto.ClaveAcceso,
            CiudadNacimiento = dto.CiudadNacimiento,
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        var clienteCreado = await _clienteRepository.Add(cliente);
        return MapToDto(clienteCreado);
    }

    public async Task<bool> ActualizarAsync(int id, ActualizarClienteDto dto)
    {
        var cliente = await _clienteRepository.GetById(id);
        if (cliente == null)
            return false;

        // Validar NIT único
        if (!string.IsNullOrEmpty(dto.Nit))
        {
            var existeNit = await _clienteRepository.ExisteNit(dto.Nit, id);  // <- Usar repositorio
            if (existeNit)
                throw new InvalidOperationException("Ya existe otro cliente con ese NIT");
        }

        // NO permitir cambiar el código
        cliente.TipoCliente = dto.TipoCliente;
        cliente.Nombre = dto.Nombre;
        cliente.NombreExtranjero = dto.NombreExtranjero;
        cliente.Grupo = dto.Grupo;
        cliente.Moneda = dto.Moneda;
        cliente.Nit = dto.Nit;
        cliente.Direccion = dto.Direccion;
        cliente.Telefono1 = dto.Telefono1;
        cliente.Telefono2 = dto.Telefono2;
        cliente.TelefonoMovil = dto.TelefonoMovil;
        cliente.Fax = dto.Fax;
        cliente.CorreoElectronico = dto.CorreoElectronico;
        cliente.SitioWeb = dto.SitioWeb;
        cliente.Posicion = dto.Posicion;
        cliente.Titulo = dto.Titulo;
        cliente.SegundoNombre = dto.SegundoNombre;
        cliente.Apellido = dto.Apellido;
        cliente.LimiteCredito = dto.LimiteCredito;
        cliente.DiasCredito = dto.DiasCredito;
        cliente.DescuentoPorcentaje = dto.DescuentoPorcentaje;
        cliente.Activo = dto.Activo;
        cliente.BloquearMarketing = dto.BloquearMarketing;
        cliente.Observaciones1 = dto.Observaciones1;
        cliente.Observaciones2 = dto.Observaciones2;
        cliente.ClaveAcceso = dto.ClaveAcceso;
        cliente.CiudadNacimiento = dto.CiudadNacimiento;
        cliente.FechaActualizacion = DateTime.UtcNow;

        return await _clienteRepository.Update(cliente);
    }

    public async Task<bool> EliminarAsync(int id)
    {
        return await _clienteRepository.Delete(id);
    }

    public async Task<IEnumerable<ClienteDto>> BuscarAsync(string? nombre, string? nit, string? codigo)
    {
        throw new NotImplementedException("Usar GetMultipleByCodigoNitOrNombre");
    }

    // Métodos de compatibilidad (alias)
    public async Task<IEnumerable<ClienteDto>> GetAll()
    {
        return await ObtenerTodosAsync();
    }

    public async Task<(IEnumerable<ClienteDto>, int)> GetWithFilters(FiltroClienteDto filtro)
    {
        (IEnumerable<Cliente> clientes, int total) = await _clienteRepository.GetWithFilters(filtro);
        var clientesDto = clientes.Select(c => MapToDto(c)).ToList();
        return (clientesDto, total);
    }

    public async Task<ClienteDto?> GetById(int id)
    {
        return await ObtenerPorIdAsync(id);
    }

    public async Task<ClienteDto?> GetByCodigoOrNit(string codigoOrNit)
    {
        var cliente = await _clienteRepository.GetByCodigo(codigoOrNit);
        if (cliente == null)
            cliente = await _clienteRepository.GetByNit(codigoOrNit);

        return cliente != null ? MapToDto(cliente) : null;
    }

    public async Task<IEnumerable<ClienteDto>> GetMultipleByCodigoNitOrNombre(string termino)
    {
        var clientes = await _clienteRepository.GetMultipleByCodigoNitOrNombre(termino);
        return clientes.Select(c => MapToDto(c)).ToList();
    }

    public async Task<ClienteDto> CreateCliente(CrearClienteDto dto, int usuarioId)
    {
        return await CrearAsync(dto);
    }

    public async Task<bool> UpdateCliente(int id, ActualizarClienteDto dto, int usuarioId)
    {
        return await ActualizarAsync(id, dto);
    }

    public async Task<bool> DisableCliente(int id, int usuarioId)
    {
        var cliente = await _clienteRepository.GetById(id);
        if (cliente == null)
            return false;

        cliente.Activo = false;
        cliente.ActualizadoPor = usuarioId;
        cliente.FechaActualizacion = DateTime.UtcNow;

        return await _clienteRepository.Update(cliente);
    }

    public async Task<bool> DeleteCliente(int id, int usuarioId)
    {
        return await EliminarAsync(id);
    }

    public async Task<bool> ExisteCodigo(string codigo)
    {
        return await _clienteRepository.ExisteCodigo(codigo);
    }

    public async Task<bool> ExisteNit(string nit)
    {
        nit = nit?.Trim().ToUpper();
        return await _clienteRepository.ExisteNit(nit);
    }

    public async Task<bool> ExisteNit(string nit, int? idExcluir)
    {
        nit = nit?.Trim().ToUpper();
        return await _clienteRepository.ExisteNit(nit, idExcluir);
    }

    // Helper privado
    private ClienteDto MapToDto(Cliente c)
    {
        return new ClienteDto
        {
            Id = c.Id,
            Codigo = c.Codigo,
            TipoCliente = c.TipoCliente,
            Nombre = c.Nombre,
            NombreExtranjero = c.NombreExtranjero,
            Grupo = c.Grupo,
            Moneda = c.Moneda,
            Nit = c.Nit,
            Direccion = c.Direccion,
            Telefono1 = c.Telefono1,
            Telefono2 = c.Telefono2,
            TelefonoMovil = c.TelefonoMovil,
            Fax = c.Fax,
            CorreoElectronico = c.CorreoElectronico,
            SitioWeb = c.SitioWeb,
            Posicion = c.Posicion,
            Titulo = c.Titulo,
            SegundoNombre = c.SegundoNombre,
            Apellido = c.Apellido,
            SaldoCuenta = c.SaldoCuenta,
            LimiteCredito = c.LimiteCredito,
            DiasCredito = c.DiasCredito,
            DescuentoPorcentaje = c.DescuentoPorcentaje,
            Activo = c.Activo,
            BloquearMarketing = c.BloquearMarketing,
            Observaciones1 = c.Observaciones1,
            Observaciones2 = c.Observaciones2,
            ClaveAcceso = c.ClaveAcceso,
            CiudadNacimiento = c.CiudadNacimiento,
            FechaCreacion = c.FechaCreacion,
            FechaActualizacion = c.FechaActualizacion,
            CreadoPor = c.CreadoPor,
            ActualizadoPor = c.ActualizadoPor
        };
    }
}