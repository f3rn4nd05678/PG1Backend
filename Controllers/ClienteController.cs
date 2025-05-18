using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Extensions;
using System.Security.Claims;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClienteController(IClienteService clienteService)
    {
        _clienteService = clienteService;
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

            var (clientes, total) = await _clienteService.GetWithFilters(filtro);

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
            return this.ApiError(ex.Message);
        }
    }

    [HttpPost("crear")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta")]
    public async Task<IActionResult> Create([FromBody] CrearClienteDto crearClienteDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cliente = await _clienteService.CreateCliente(crearClienteDto, userId);

            return this.ApiCreated(cliente, "Cliente creado exitosamente");
        }
        catch (Exception ex)
        {
            return this.ApiError(ex.Message);
        }
    }

    [HttpPost("actualizar")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta")]
    public async Task<IActionResult> Update([FromBody] ActualizarClienteRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cliente = await _clienteService.UpdateCliente(request.Id, request.Cliente, userId);

            return this.ApiOk(cliente, "Cliente actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return this.ApiError(ex.Message);
        }
    }

    [HttpPost("eliminar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete([FromBody] EliminarClienteDto request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _clienteService.DeleteCliente(request.Id, userId);

            return this.ApiOk("Cliente eliminado exitosamente");
        }
        catch (Exception ex)
        {
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
            return this.ApiError(ex.Message);
        }
    }

    [HttpPost("validar-nit")]
    [Authorize(Roles = "Administrador,Vendedor,Punto de venta")]
    public async Task<IActionResult> ValidarNit([FromBody] ValidarNitDto request)
    {
        try
        {
            var existe = await _clienteService.ExisteNit(request.Nit);
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
            return this.ApiError(ex.Message);
        }
    }
}