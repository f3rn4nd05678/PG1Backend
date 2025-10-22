using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Extensions;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(IUsuarioService usuarioService, ILogger<UsuarioController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    /// <summary>
    /// Listar usuarios con filtros y paginación
    /// </summary>
    [HttpPost("listar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Listar([FromBody] FiltroUsuarioDto? filtro = null)
    {
        try
        {
            filtro ??= new FiltroUsuarioDto();

            if (filtro.ElementosPorPagina <= 0)
                filtro.ElementosPorPagina = 10;

            var (usuarios, total) = await _usuarioService.GetWithFilters(filtro);

            var response = new ListadoUsuariosResponseDto
            {
                Usuarios = usuarios,
                Total = total,
                Pagina = filtro.Pagina,
                ElementosPorPagina = filtro.ElementosPorPagina,
                TotalPaginas = (int)Math.Ceiling((double)total / filtro.ElementosPorPagina)
            };

            return this.ApiOk(response, "Usuarios obtenidos correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar usuarios");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Obtener usuario por ID
    /// </summary>
    [HttpPost("obtener")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Obtener([FromBody] ObtenerUsuarioPorIdDto request)
    {
        try
        {
            var usuario = await _usuarioService.GetById(request.Id);
            if (usuario == null)
                return this.ApiNotFound("Usuario no encontrado");

            return this.ApiOk(usuario, "Usuario obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Crear nuevo usuario
    /// </summary>
    [HttpPost("crear")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] RegistroUsuarioDto usuarioDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return this.ApiError("Datos inválidos", 400);

            var usuario = await _usuarioService.Registrar(usuarioDto);
            return this.ApiCreated(usuario, "Usuario creado exitosamente");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al crear usuario");
            return this.ApiError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Actualizar usuario existente
    /// </summary>
    [HttpPost("actualizar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar([FromBody] ActualizarUsuarioRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return this.ApiError("Datos inválidos", 400);

            if (request.Id <= 0)
                return this.ApiError("ID de usuario inválido");

            await _usuarioService.Update(request.Id, request.Datos);

            var usuarioActualizado = await _usuarioService.GetById(request.Id);
            return this.ApiOk(usuarioActualizado, "Usuario actualizado exitosamente");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al actualizar usuario");
            return this.ApiError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Eliminar usuario
    /// </summary>
    [HttpPost("eliminar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar([FromBody] EliminarUsuarioDto request)
    {
        try
        {
            await _usuarioService.Delete(request.Id);
            return this.ApiOk(null, "Usuario eliminado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Validar si un correo ya existe
    /// </summary>
    [HttpPost("validar-correo")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ValidarCorreo([FromBody] ValidarCorreoDto request)
    {
        try
        {
            var existe = await _usuarioService.ExisteCorreo(request.Correo);

            // Si se proporciona un ID a excluir (para edición)
            if (existe && request.IdExcluir.HasValue)
            {
                var usuarioExistente = await _usuarioService.GetByCorreo(request.Correo);
                existe = usuarioExistente != null && usuarioExistente.Id != request.IdExcluir.Value;
            }

            return this.ApiOk(new { existe }, existe ? "El correo ya está registrado" : "Correo disponible");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar correo");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Cambiar contraseña de usuario
    /// </summary>
    [HttpPost("cambiar-password")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDto request)
    {
        try
        {
            await _usuarioService.CambiarPassword(request.Id, request.NuevaPassword);
            return this.ApiOk(null, "Contraseña actualizada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar contraseña");
            return this.ApiError(ex.Message);
        }
    }

    /// <summary>
    /// Activar o desactivar usuario
    /// </summary>
    [HttpPost("activar-desactivar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ActivarDesactivar([FromBody] ActivarDesactivarUsuarioDto request)
    {
        try
        {
            await _usuarioService.ActivarDesactivar(request.Id, request.Activo);
            var mensaje = request.Activo ? "Usuario activado exitosamente" : "Usuario desactivado exitosamente";
            return this.ApiOk(null, mensaje);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al activar/desactivar usuario");
            return this.ApiError(ex.Message);
        }
    }
}