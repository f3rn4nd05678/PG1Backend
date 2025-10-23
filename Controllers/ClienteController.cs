using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Extensions;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Services.IServices;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly ILogger<ClienteController> _logger;

    public ClienteController(IClienteService clienteService, ILogger<ClienteController> logger)
    {
        _clienteService = clienteService;
        _logger = logger;
    }

    [HttpGet("listar")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta")]
    public async Task<IActionResult> GetAllClientes()
    {
        try
        {
            var clientes = await _clienteService.GetAll();

            var response = new
            {
                Clientes = clientes,
                Total = clientes.Count(),
                Mensaje = "Clientes obtenidos correctamente"
            };

            return this.ApiOk(response, "Clientes obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los clientes");
            return this.ApiError(ex.Message);
        }
    }

    [HttpPost("listar")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta")]
    public async Task<IActionResult> GetAll([FromBody] FiltroClienteDto? filtro = null)
    {
        try
        {
            filtro ??= new FiltroClienteDto();

            if (filtro.ElementosPorPagina <= 0)
                filtro.ElementosPorPagina = 10;

            var resultado = await _clienteService.GetWithFilters(filtro);
            var clientes = resultado.Item1.ToList();
            var total = resultado.Item2;

            var response = new ListadoClientesResponseDto
            {
                Clientes = clientes,
                Total = total,
                Pagina = filtro.Pagina,
                ElementosPorPagina = filtro.ElementosPorPagina,
                TotalPaginas = (int)Math.Ceiling((double)total / filtro.ElementosPorPagina)
            };

            return this.ApiOk(response, "Clientes obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener clientes con filtros");
            return this.ApiError(ex.Message);
        }
    }

    [HttpPost("obtener")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta")]
    public async Task<IActionResult> GetById([FromBody] ObtenerClientePorIdDto request)
    {
        try
        {
            var cliente = await _clienteService.GetById(request.Id);
            if (cliente == null)
                return this.ApiNotFound("Cliente no encontrado");

            return this.ApiOk(cliente, "Cliente obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cliente por ID: {Id}", request.Id);
            return this.ApiError(ex.Message);
        }
    }

    [HttpPost("buscar")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta")]
    public async Task<IActionResult> GetByCodigoNitOrNombre([FromBody] BuscarClienteDto request)
    {
        try
        {
            var clientes = await _clienteService.GetMultipleByCodigoNitOrNombre(request.TerminoBusqueda);

            if (!clientes.Any())
                return this.ApiNotFound("No se encontraron clientes");

            if (clientes.Count() == 1)
                return this.ApiOk(clientes.First(), "Cliente encontrado");

            return this.ApiOk(new
            {
                resultados = clientes,
                total = clientes.Count(),
                mensaje = $"Se encontraron {clientes.Count()} clientes"
            }, "Múltiples clientes encontrados");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar clientes con término: {Termino}", request.TerminoBusqueda);
            return this.ApiError(ex.Message);
        }
    }

    [HttpPost("crear")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta")]
    public async Task<IActionResult> Create([FromBody] CrearClienteDto crearClienteDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(crearClienteDto.Nombre))
            {
                return this.ApiError("El nombre del cliente es obligatorio");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return this.ApiError("Usuario no válido");
            }

            var cliente = await _clienteService.CreateCliente(crearClienteDto, userId);
            return this.ApiCreated(cliente, "Cliente creado exitosamente");
        }
        catch (DbUpdateException dbEx)
        {
            var detail = dbEx.InnerException?.Message ?? dbEx.Message;
            _logger.LogError(dbEx, "DB error al crear cliente: {Detail}", detail);
            return this.ApiError($"DB error: {detail}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear cliente");
            return this.ApiError(ex.Message);
        }
    }

    [HttpPost("actualizar")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta")]
    public async Task<IActionResult> Update([FromBody] ActualizarClienteRequest request)
    {
        try
        {

            if (request == null || request.Cliente == null)
            {
                return this.ApiError("Datos de actualización inválidos");
            }

            if (request.Id <= 0)
            {
                return this.ApiError("ID de cliente inválido");
            }

            if (string.IsNullOrWhiteSpace(request.Cliente.Nombre))
            {
                return this.ApiError("El nombre del cliente es obligatorio");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return this.ApiError("Usuario no válido");
            }

            _logger.LogInformation("Iniciando actualización de cliente ID: {Id} por usuario: {UserId}", request.Id, userId);

            var cliente = await _clienteService.UpdateCliente(request.Id, request.Cliente, userId);

            return this.ApiOk(cliente, "Cliente actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cliente ID: {Id}. Error: {Message}", request?.Id, ex.Message);
            return this.ApiError($"Error al actualizar el cliente: {ex.Message}");
        }
    }

    [HttpPost("eliminar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Disable([FromBody] EliminarClienteDto request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return this.ApiError("Usuario no válido");

            await _clienteService.DisableCliente(request.Id, userId);

            return this.ApiOk("Cliente deshabilitado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al deshabilitar cliente ID: {Id}", request.Id);
            return this.ApiError(ex.Message);
        }
    }

    [HttpPost("validar-codigo")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta")]
    public async Task<IActionResult> ValidarCodigo([FromBody] ValidarCodigoDto request)
    {
        try
        {
            var existe = await _clienteService.ExisteCodigo(request.Codigo);
            var response = new ValidacionResponseDto
            {
                Existe = existe,
                Disponible = !existe,
                Mensaje = existe ? "El código ya está en uso" : "El código está disponible"
            };

            return this.ApiOk(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar código: {Codigo}", request.Codigo);
            return this.ApiError(ex.Message);
        }
    }

    [HttpPost("validar-nit")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta")]
    public async Task<IActionResult> ValidarNit([FromBody] ValidarNitDto request)
    {
        try
        {
            var existe = await _clienteService.ExisteNit(request.Nit,null);
            var response = new ValidacionResponseDto
            {
                Existe = existe,
                Disponible = !existe,
                Mensaje = existe ? "El NIT ya está en uso" : "El NIT está disponible"
            };

            return this.ApiOk(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar NIT: {Nit}", request.Nit);
            return this.ApiError(ex.Message);
        }
    }
}