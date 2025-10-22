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

    public AuthController(IUsuarioService usuarioService, IUsuarioRepository usuarioRepository)
    {
        _usuarioService = usuarioService;
        _usuarioRepository = usuarioRepository;
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
}