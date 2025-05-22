using ProyectoGraduación.Models;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.IServices;
using System.Linq;

namespace ProyectoGraduación.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ClienteDto>> GetAll()
    {
        var clientes = await _repository.GetAll();
        return clientes.Select(MapToDto);
    }

    public async Task<(IEnumerable<ClienteDto> clientes, int total)> GetWithFilters(FiltroClienteDto filtro)
    {
        var (clientes, total) = await _repository.GetWithFilters(filtro);
        return (clientes.Select(MapToDto), total);
    }

    public async Task<ClienteDto?> GetById(int id)
    {
        var cliente = await _repository.GetById(id);
        return cliente != null ? MapToDto(cliente) : null;
    }

    public async Task<IEnumerable<ClienteDto>> GetMultipleByCodigoNitOrNombre(string terminoBusqueda)
    {
        var clientes = await _repository.GetMultipleByCodigoNitOrNombre(terminoBusqueda);
        return clientes.Select(MapToDto);
    }

    public async Task<ClienteDto> CreateCliente(CrearClienteDto crearClienteDto, int usuarioId)
    {
        // Validaciones
        if (!string.IsNullOrEmpty(crearClienteDto.Nit) && await _repository.ExisteNit(crearClienteDto.Nit))
        {
            throw new Exception("Ya existe un cliente con ese NIT");
        }

        // Generar código si no se proporcionó
        var codigo = string.IsNullOrEmpty(crearClienteDto.Codigo)
            ? await _repository.GenerateNextCodigo()
            : crearClienteDto.Codigo;

        if (await _repository.ExisteCodigo(codigo))
        {
            throw new Exception("Ya existe un cliente con ese código");
        }

        var cliente = new Cliente
        {
            Codigo = codigo,
            TipoCliente = crearClienteDto.TipoCliente,
            Nombre = crearClienteDto.Nombre,
            NombreExtranjero = crearClienteDto.NombreExtranjero,
            Grupo = crearClienteDto.Grupo,
            Moneda = crearClienteDto.Moneda,
            Nit = crearClienteDto.Nit,
            Direccion = crearClienteDto.Direccion,
            Telefono1 = crearClienteDto.Telefono1,
            Telefono2 = crearClienteDto.Telefono2,
            TelefonoMovil = crearClienteDto.TelefonoMovil,
            Fax = crearClienteDto.Fax,
            CorreoElectronico = crearClienteDto.CorreoElectronico,
            SitioWeb = crearClienteDto.SitioWeb,
            Posicion = crearClienteDto.Posicion,
            Titulo = crearClienteDto.Titulo,
            SegundoNombre = crearClienteDto.SegundoNombre,
            Apellido = crearClienteDto.Apellido,
            LimiteCredito = crearClienteDto.LimiteCredito,
            DiasCredito = crearClienteDto.DiasCredito,
            DescuentoPorcentaje = crearClienteDto.DescuentoPorcentaje,
            BloquearMarketing = crearClienteDto.BloquearMarketing,
            Observaciones1 = crearClienteDto.Observaciones1,
            Observaciones2 = crearClienteDto.Observaciones2,
            ClaveAcceso = crearClienteDto.ClaveAcceso,
            CiudadNacimiento = crearClienteDto.CiudadNacimiento,
            CreadoPor = usuarioId,
            ActualizadoPor = usuarioId
        };

        await _repository.Add(cliente);

        // Obtener el cliente creado con información completa
        var clienteCreado = await _repository.GetById(cliente.Id);
        return MapToDto(clienteCreado!);
    }

    public async Task<ClienteDto> UpdateCliente(int id, ActualizarClienteDto actualizarClienteDto, int usuarioId)
    {
        var clienteExistente = await _repository.GetById(id);
        if (clienteExistente == null)
        {
            throw new Exception("Cliente no encontrado");
        }

        // Validar NIT si cambió
        if (!string.IsNullOrEmpty(actualizarClienteDto.Nit) &&
            actualizarClienteDto.Nit != clienteExistente.Nit &&
            await _repository.ExisteNit(actualizarClienteDto.Nit))
        {
            throw new Exception("Ya existe otro cliente con ese NIT");
        }

        // Validar código si cambió
        if (!string.IsNullOrEmpty(actualizarClienteDto.Codigo) &&
            actualizarClienteDto.Codigo != clienteExistente.Codigo &&
            await _repository.ExisteCodigo(actualizarClienteDto.Codigo))
        {
            throw new Exception("Ya existe otro cliente con ese código");
        }

        // Actualizar propiedades
        clienteExistente.Codigo = actualizarClienteDto.Codigo ?? clienteExistente.Codigo;
        clienteExistente.TipoCliente = actualizarClienteDto.TipoCliente;
        clienteExistente.Nombre = actualizarClienteDto.Nombre;
        clienteExistente.NombreExtranjero = actualizarClienteDto.NombreExtranjero;
        clienteExistente.Grupo = actualizarClienteDto.Grupo;
        clienteExistente.Moneda = actualizarClienteDto.Moneda;
        clienteExistente.Nit = actualizarClienteDto.Nit;
        clienteExistente.Direccion = actualizarClienteDto.Direccion;
        clienteExistente.Telefono1 = actualizarClienteDto.Telefono1;
        clienteExistente.Telefono2 = actualizarClienteDto.Telefono2;
        clienteExistente.TelefonoMovil = actualizarClienteDto.TelefonoMovil;
        clienteExistente.Fax = actualizarClienteDto.Fax;
        clienteExistente.CorreoElectronico = actualizarClienteDto.CorreoElectronico;
        clienteExistente.SitioWeb = actualizarClienteDto.SitioWeb;
        clienteExistente.Posicion = actualizarClienteDto.Posicion;
        clienteExistente.Titulo = actualizarClienteDto.Titulo;
        clienteExistente.SegundoNombre = actualizarClienteDto.SegundoNombre;
        clienteExistente.Apellido = actualizarClienteDto.Apellido;
        clienteExistente.LimiteCredito = actualizarClienteDto.LimiteCredito;
        clienteExistente.DiasCredito = actualizarClienteDto.DiasCredito;
        clienteExistente.DescuentoPorcentaje = actualizarClienteDto.DescuentoPorcentaje;
        clienteExistente.BloquearMarketing = actualizarClienteDto.BloquearMarketing;
        clienteExistente.Observaciones1 = actualizarClienteDto.Observaciones1;
        clienteExistente.Observaciones2 = actualizarClienteDto.Observaciones2;
        clienteExistente.ClaveAcceso = actualizarClienteDto.ClaveAcceso;
        clienteExistente.CiudadNacimiento = actualizarClienteDto.CiudadNacimiento;
        clienteExistente.Activo = actualizarClienteDto.Activo;
        clienteExistente.ActualizadoPor = usuarioId;

        await _repository.Update(clienteExistente);

        return MapToDto(clienteExistente);
    }

    public async Task DeleteCliente(int id, int usuarioId)
    {
        var cliente = await _repository.GetById(id);
        if (cliente == null)
        {
            throw new Exception("Cliente no encontrado");
        }

        cliente.ActualizadoPor = usuarioId;
        await _repository.Delete(id);
    }

    public async Task<bool> ExisteCodigo(string codigo)
    {
        return await _repository.ExisteCodigo(codigo);
    }

    public async Task<bool> ExisteNit(string nit)
    {
        return await _repository.ExisteNit(nit);
    }

    private ClienteDto MapToDto(Cliente cliente)
    {
        return new ClienteDto
        {
            Id = cliente.Id,
            Codigo = cliente.Codigo,
            TipoCliente = cliente.TipoCliente,
            Nombre = cliente.Nombre,
            NombreExtranjero = cliente.NombreExtranjero,
            Grupo = cliente.Grupo,
            Moneda = cliente.Moneda,
            Nit = cliente.Nit,
            Direccion = cliente.Direccion,

            // Información de contacto
            Telefono1 = cliente.Telefono1,
            Telefono2 = cliente.Telefono2,
            TelefonoMovil = cliente.TelefonoMovil,
            Fax = cliente.Fax,
            CorreoElectronico = cliente.CorreoElectronico,
            SitioWeb = cliente.SitioWeb,

            // Información personal
            Posicion = cliente.Posicion,
            Titulo = cliente.Titulo,
            SegundoNombre = cliente.SegundoNombre,
            Apellido = cliente.Apellido,

            // Campos financieros
            SaldoCuenta = cliente.SaldoCuenta,
            LimiteCredito = cliente.LimiteCredito,
            DiasCredito = cliente.DiasCredito,
            DescuentoPorcentaje = cliente.DescuentoPorcentaje,

            // Configuraciones
            Activo = cliente.Activo,
            BloquearMarketing = cliente.BloquearMarketing,
            Observaciones1 = cliente.Observaciones1,
            Observaciones2 = cliente.Observaciones2,
            ClaveAcceso = cliente.ClaveAcceso,
            CiudadNacimiento = cliente.CiudadNacimiento,

            // Auditoría
            FechaCreacion = cliente.FechaCreacion,
            FechaActualizacion = cliente.FechaActualizacion,
            CreadoPor = cliente.CreadoPor,
            ActualizadoPor = cliente.ActualizadoPor
        };
    }

    public async Task<ClienteDto?> GetByCodigoOrNit(string codigoOrNit)
    {
        var cliente = await _repository.GetByCodigoOrNit(codigoOrNit);
        return cliente != null ? MapToDto(cliente) : null;
    }
}