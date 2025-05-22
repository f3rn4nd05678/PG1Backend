using ProyectoGraduación.Models;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.IServices;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ProyectoGraduación.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(IClienteRepository repository, ILogger<ClienteService> logger)
    {
        _repository = repository;
        _logger = logger;
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
        try
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
                ActualizadoPor = usuarioId,
                Activo = true
            };

            await _repository.Add(cliente);

            // Obtener el cliente creado con información completa
            var clienteCreado = await _repository.GetById(cliente.Id);
            return MapToDto(clienteCreado!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear cliente: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<ClienteDto> UpdateCliente(int id, ActualizarClienteDto actualizarClienteDto, int usuarioId)
    {
        try
        {
            var clienteExistente = await _repository.GetById(id);
            if (clienteExistente == null)
            {
                throw new Exception("Cliente no encontrado");
            }

            _logger.LogInformation("Actualizando cliente ID: {Id}, Usuario: {UserId}", id, usuarioId);

            // Validar NIT si cambió y no está vacío
            if (!string.IsNullOrEmpty(actualizarClienteDto.Nit) &&
                actualizarClienteDto.Nit != clienteExistente.Nit)
            {
                var existeNit = await _repository.ExisteNit(actualizarClienteDto.Nit);
                if (existeNit)
                {
                    throw new Exception("Ya existe otro cliente con ese NIT");
                }
            }

            // Validar código si cambió y no está vacío
            if (!string.IsNullOrEmpty(actualizarClienteDto.Codigo) &&
                actualizarClienteDto.Codigo != clienteExistente.Codigo)
            {
                var existeCodigo = await _repository.ExisteCodigo(actualizarClienteDto.Codigo);
                if (existeCodigo)
                {
                    throw new Exception("Ya existe otro cliente con ese código");
                }
            }

            // Actualizar propiedades - usar valores existentes si los nuevos están vacíos
            clienteExistente.Codigo = !string.IsNullOrEmpty(actualizarClienteDto.Codigo)
                ? actualizarClienteDto.Codigo
                : clienteExistente.Codigo;

            clienteExistente.TipoCliente = actualizarClienteDto.TipoCliente ?? clienteExistente.TipoCliente;
            clienteExistente.Nombre = actualizarClienteDto.Nombre ?? clienteExistente.Nombre;
            clienteExistente.NombreExtranjero = actualizarClienteDto.NombreExtranjero;
            clienteExistente.Grupo = actualizarClienteDto.Grupo;
            clienteExistente.Moneda = actualizarClienteDto.Moneda ?? clienteExistente.Moneda;
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
            // La fecha de actualización se maneja en el repositorio

            await _repository.Update(clienteExistente);

            _logger.LogInformation("Cliente actualizado exitosamente ID: {Id}", id);
            return MapToDto(clienteExistente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cliente ID: {Id}. Error: {Message}", id, ex.Message);

            // Si es un error de validación conocido, relanzar tal como está
            if (ex.Message.Contains("Ya existe"))
            {
                throw;
            }

            // Para otros errores, proporcionar más contexto
            throw new Exception($"Error al actualizar el cliente: {ex.Message}", ex);
        }
    }

    public async Task DeleteCliente(int id, int usuarioId)
    {
        try
        {
            var cliente = await _repository.GetById(id);
            if (cliente == null)
            {
                throw new Exception("Cliente no encontrado");
            }

            // Eliminar completamente (hard delete)
            await _repository.Delete(id);

            _logger.LogInformation("Cliente eliminado por usuario {UserId}: Cliente ID {Id}", usuarioId, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar cliente ID: {Id}", id);
            throw;
        }
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