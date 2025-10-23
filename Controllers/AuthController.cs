using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IServices;
using ProyectoGraduación.Models;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.Extensions;

namespace ProyectoGraduación.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUsuarioService usuarioService, IUsuarioRepository usuarioRepository, ILogger<AuthController> logger)
    {
        _usuarioService = usuarioService;
        _usuarioRepository = usuarioRepository;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var loginResponse = await _usuarioService.Login(loginDto);

            return this.ApiOk(loginResponse, "Inicio de sesión exitoso");
        }
        catch (Exception ex)
        {
            return this.ApiUnauthorized(ex.Message);
        }
    }

    [HttpPost("crear-usuario")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CrearUsuario([FromBody] RegistroUsuarioDto model)
    {
        try
        {
            var usuarioExistente = await _usuarioRepository.GetByCorreo(model.Correo);
            if (usuarioExistente != null)
                return this.ApiError("El correo ya está registrado");


            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);


            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                Correo = model.Correo,
                Password = hashedPassword,
                RolId = model.RolId
            };

            await _usuarioRepository.Add(usuario);

            var usuarioCreado = await _usuarioRepository.GetByCorreo(model.Correo);

            var resultado = new
            {
                id = usuarioCreado.Id,
                nombre = usuarioCreado.Nombre,
                correo = usuarioCreado.Correo,
                rolId = usuarioCreado.RolId
            };

            return this.ApiCreated(resultado, "Usuario creado exitosamente");
        }
        catch (Exception ex)
        {
            return this.ApiError(ex.Message);
        }
    }


    [HttpPost("reiniciar-contrasenia")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByCorreo(model.Correo);
            if (usuario == null)
                return this.ApiNotFound("Usuario no encontrado");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            usuario.Password = hashedPassword;
            await _usuarioRepository.Update(usuario);

            return this.ApiOk("Contraseña actualizada correctamente");
        }
        catch (Exception ex)
        {
            return this.ApiError(ex.Message);
        }
    }

    [HttpPost("listar-usuarios")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ListarUsuarios()
    {
        try
        {
            var usuarios = await _usuarioService.GetAll();
            return this.ApiOk(usuarios, "Usuarios obtenidos correctamente");
        }
        catch (Exception ex)
        {
            return this.ApiError(ex.Message);
        }
    }

    [HttpPost("solicitar-reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> SolicitarResetPassword([FromBody] SolicitarResetPasswordDto request)
    {
        try
        {
            _logger.LogInformation("Solicitud de reset password recibida para: {Correo}", request?.Correo);

            if (request == null || string.IsNullOrWhiteSpace(request.Correo))
            {
                _logger.LogWarning("Solicitud inválida - correo vacío");
                return this.ApiError("El correo es requerido");
            }

            var resultado = await _usuarioService.SolicitarResetPassword(request.Correo);

            if (!resultado)
            {
                _logger.LogInformation("Correo no encontrado: {Correo}", request.Correo);
                return this.ApiError("El correo ingresado no está registrado en el sistema.");
            }

            _logger.LogInformation("Reset password exitoso para: {Correo}", request.Correo);
            return this.ApiOk<object>(
                null,
                "Se ha enviado un correo con instrucciones para restablecer tu contraseña."
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar solicitud de reset password para {Correo}", request?.Correo);
            return this.ApiError($"Ocurrió un error al procesar tu solicitud: {ex.Message}");
        }
    }
}