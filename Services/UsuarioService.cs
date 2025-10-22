using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProyectoGraduación.Models;
using ProyectoGraduación.DTOs;
using ProyectoGraduación.IRepositories;
using ProyectoGraduación.IServices;
using System.Linq;

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

    public async Task<(IEnumerable<UsuarioDto> usuarios, int total)> GetWithFilters(FiltroUsuarioDto filtro)
    {
        var todosUsuarios = await _repository.GetAll();
        var query = todosUsuarios.AsQueryable();

        // Filtro por término de búsqueda (nombre o correo)
        if (!string.IsNullOrWhiteSpace(filtro.TerminoBusqueda))
        {
            var termino = filtro.TerminoBusqueda.ToLower();
            query = query.Where(u =>
                u.Nombre.ToLower().Contains(termino) ||
                u.Correo.ToLower().Contains(termino)
            );
        }

        // Filtro por rol
        if (filtro.RolId.HasValue)
        {
            query = query.Where(u => u.RolId == filtro.RolId.Value);
        }

        // Filtro por activo
        if (filtro.Activo.HasValue)
        {
            query = query.Where(u => u.Activo == filtro.Activo.Value);
        }

        var total = query.Count();

        // Paginación
        var usuarios = query
            .Skip((filtro.Pagina - 1) * filtro.ElementosPorPagina)
            .Take(filtro.ElementosPorPagina)
            .Select(MapToDto)
            .ToList();

        return (usuarios, total);
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
            throw new InvalidOperationException("El correo ya está registrado");

        var usuario = new Usuario
        {
            Nombre = usuarioDto.Nombre,
            Correo = usuarioDto.Correo,
            Password = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Password),
            RolId = usuarioDto.RolId,
            Activo = true,
            ForzarCambioPassword = usuarioDto.ForzarCambioPassword ?? false
        };

        await _repository.Add(usuario);

        var usuarioCreado = await _repository.GetByCorreo(usuario.Correo);
        return MapToDto(usuarioCreado);
    }

    public async Task<string> Login(LoginDto loginDto)
    {
        var usuario = await _repository.GetByCorreo(loginDto.Correo);

        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        if (!usuario.Activo)
            throw new InvalidOperationException("El usuario está inactivo");

        try
        {
            bool passwordCoincide = BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.Password);
            if (!passwordCoincide)
                throw new InvalidOperationException("Contraseña incorrecta");
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Error en la verificación: {ex.Message}");
        }

        // Actualizar último login
        usuario.UltimoLogin = DateTime.UtcNow;
        await _repository.Update(usuario);

        return GenerarToken(usuario);
    }

    public async Task Update(int id, RegistroUsuarioDto usuarioDto)
    {
        var usuario = await _repository.GetById(id);

        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        if (usuario.Correo != usuarioDto.Correo && await _repository.ExisteCorreo(usuarioDto.Correo))
            throw new InvalidOperationException("El correo ya está registrado por otro usuario");

        usuario.Nombre = usuarioDto.Nombre;
        usuario.Correo = usuarioDto.Correo;
        usuario.RolId = usuarioDto.RolId;

        if (usuarioDto.ForzarCambioPassword.HasValue)
            usuario.ForzarCambioPassword = usuarioDto.ForzarCambioPassword.Value;

        // Solo actualizar password si se proporciona uno nuevo
        if (!string.IsNullOrEmpty(usuarioDto.Password))
        {
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Password);
        }

        await _repository.Update(usuario);
    }

    public async Task Delete(int id)
    {
        var usuario = await _repository.GetById(id);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        // Verificar que no sea el último administrador
        var usuarios = await _repository.GetAll();
        var adminRole = usuarios.FirstOrDefault(u => u.Rol?.Nombre == "Administrador" && u.Id == id);

        if (adminRole != null)
        {
            var adminsCount = usuarios.Count(u => u.Rol?.Nombre == "Administrador" && u.Activo);
            if (adminsCount <= 1)
                throw new InvalidOperationException("No se puede eliminar el último administrador del sistema");
        }

        await _repository.Delete(id);
    }

    public async Task<bool> ExisteCorreo(string correo)
    {
        return await _repository.ExisteCorreo(correo);
    }

    public async Task<IEnumerable<UsuarioDto>> BuscarPorRol(int rolId)
    {
        var usuarios = await _repository.GetByRol(rolId);
        return usuarios.Select(MapToDto);
    }

    public async Task CambiarPassword(int id, string nuevaPassword)
    {
        var usuario = await _repository.GetById(id);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        usuario.Password = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);
        usuario.ForzarCambioPassword = false;

        await _repository.Update(usuario);
    }

    public async Task ActivarDesactivar(int id, bool activo)
    {
        var usuario = await _repository.GetById(id);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        // Verificar que no sea el último administrador activo
        if (!activo && usuario.Rol?.Nombre == "Administrador")
        {
            var usuarios = await _repository.GetAll();
            var adminsActivos = usuarios.Count(u => u.Rol?.Nombre == "Administrador" && u.Activo);

            if (adminsActivos <= 1)
                throw new InvalidOperationException("No se puede desactivar el último administrador del sistema");
        }

        usuario.Activo = activo;
        await _repository.Update(usuario);
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
            _configuration["Jwt:Key"] ?? "Plastihogar2025$"));
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
            RolNombre = usuario.Rol?.Nombre,
            Activo = usuario.Activo,
            UltimoLogin = usuario.UltimoLogin,
            ForzarCambioPassword = usuario.ForzarCambioPassword
        };
    }
}