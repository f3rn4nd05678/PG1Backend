using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProyectoGraduación.Models;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.IServices;

namespace ProyectoGraduación.Services;
public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly IConfiguration _configuration;

    public UsuarioService(IUsuarioRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<IEnumerable<UsuarioDto>> GetAll()
    {
        var usuarios = await _repository.GetAll();
        return usuarios.Select(MapToDto);
    }

    public async Task<UsuarioDto> GetById(int id)
    {
        var usuario = await _repository.GetById(id);
        return usuario != null ? MapToDto(usuario) : null;
    }

    public async Task<UsuarioDto> GetByCorreo(string correo)
    {
        var usuario = await _repository.GetByCorreo(correo);
        return usuario != null ? MapToDto(usuario) : null;
    }

    public async Task<UsuarioDto> Registrar(RegistroUsuarioDto usuarioDto)
    {
        if (await _repository.ExisteCorreo(usuarioDto.Correo))
            throw new Exception("El correo ya está registrado");

        var usuario = new Usuario
        {
            Nombre = usuarioDto.Nombre,
            Correo = usuarioDto.Correo,
            Password = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Password),
            RolId = usuarioDto.RolId
        };

        await _repository.Add(usuario);

        // Recuperamos el usuario para obtener su ID y datos de rol
        var usuarioCreado = await _repository.GetByCorreo(usuario.Correo);
        return MapToDto(usuarioCreado);
    }

    public async Task<string> Login(LoginDto loginDto)
    {
        var usuario = await _repository.GetByCorreo(loginDto.Correo);

        if (usuario == null)
            throw new Exception("Usuario no encontrado");

        try
        {
            bool passwordCoincide = BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.Password);
            if (!passwordCoincide)
                throw new Exception("Contraseña incorrecta");
        }
        catch (Exception ex)
        {
            // Log detallado del error para diagnóstico
            Console.WriteLine($"Error verificando contraseña: {ex.Message}");
            throw new Exception($"Error en la verificación: {ex.Message}");
        }

        return GenerarToken(usuario);
    }

    public async Task Update(int id, RegistroUsuarioDto usuarioDto)
    {
        var usuario = await _repository.GetById(id);

        if (usuario == null)
            throw new Exception("Usuario no encontrado");

        // Verificar si el correo ya existe y no es del usuario actual
        if (usuario.Correo != usuarioDto.Correo && await _repository.ExisteCorreo(usuarioDto.Correo))
            throw new Exception("El correo ya está registrado por otro usuario");

        usuario.Nombre = usuarioDto.Nombre;
        usuario.Correo = usuarioDto.Correo;

        // Solo actualizar password si se proporciona una nueva
        if (!string.IsNullOrEmpty(usuarioDto.Password))
        {
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Password);
        }

        usuario.RolId = usuarioDto.RolId;

        await _repository.Update(usuario);
    }

    public async Task Delete(int id)
    {
        await _repository.Delete(id);
    }

    public async Task<bool> ExisteCorreo(string correo)
    {
        return await _repository.ExisteCorreo(correo);
    }

    private string GenerarToken(Usuario usuario)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
        new Claim(ClaimTypes.Name, usuario.Nombre),
        new Claim(ClaimTypes.Email, usuario.Correo),
        new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? "Sin Rol")
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "ClaveSecretaPorDefectoMuySegura12345"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private UsuarioDto MapToDto(Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            RolId = usuario.RolId,
            RolNombre = usuario.Rol?.Nombre
        };
    }

    public async Task<IEnumerable<UsuarioDto>> BuscarPorRol(int rolId)
    {
        // Implementar en el repositorio primero
        var usuarios = await _repository.GetByRol(rolId);
        return usuarios.Select(MapToDto);
    }

}