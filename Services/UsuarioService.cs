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
    private readonly IPasswordGeneratorService _passwordGenerator;
    private readonly IEmailService _emailService;
    private readonly ILogger<UsuarioService> _logger;
    private readonly IConfiguration _configuration;


    public UsuarioService(
        IUsuarioRepository repository,
        IPasswordGeneratorService passwordGenerator,
        IEmailService emailService,
        ILogger<UsuarioService> logger,
        IConfiguration configuration)
    {
        _repository = repository;
        _passwordGenerator = passwordGenerator;
        _emailService = emailService;
        _logger = logger;
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

        // Generar contraseña temporal SIEMPRE (ignorar si viene password)
        string passwordTemporal = _passwordGenerator.GenerarPasswordTemporal();

        _logger.LogInformation("Generando contraseña temporal para usuario {Correo}", usuarioDto.Correo);

        var usuario = new Usuario
        {
            Nombre = usuarioDto.Nombre,
            Correo = usuarioDto.Correo,
            Password = BCrypt.Net.BCrypt.HashPassword(passwordTemporal),
            RolId = usuarioDto.RolId,
            Activo = usuarioDto.Activo,
            ForzarCambioPassword = true,
            UltimoLogin = null
        };

        await _repository.Add(usuario);

        var usuarioCreado = await _repository.GetByCorreo(usuario.Correo);

        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.EnviarCredencialesNuevoUsuario(
                    usuarioCreado.Correo,
                    usuarioCreado.Nombre,
                    passwordTemporal
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo de bienvenida a {Correo}", usuarioCreado.Correo);
            }
        });

        return MapToDto(usuarioCreado);
    }

    public async Task<LoginResponseDto> Login(LoginDto loginDto)
    {
        var usuario = await _repository.GetByCorreo(loginDto.Correo);

        if (usuario == null)
            throw new UnauthorizedAccessException("Credenciales inválidas");

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.Password))
            throw new UnauthorizedAccessException("Credenciales inválidas");

        if (!usuario.Activo)
            throw new UnauthorizedAccessException("Usuario inactivo");

        if (usuario.Rol == null)
        {
            _logger.LogError("Usuario {Correo} no tiene rol cargado", usuario.Correo);
            throw new InvalidOperationException("Error en la configuración del usuario. Contacte al administrador.");
        }

        if (!usuario.ForzarCambioPassword)
        {
            usuario.UltimoLogin = DateTime.UtcNow;
            await _repository.Update(usuario);
        }

        var token = GenerarToken(usuario);

        return new LoginResponseDto
        {
            Token = token,
            Usuario = MapToDto(usuario),
            RequirePasswordChange = usuario.ForzarCambioPassword
        };
    }

    public async Task Update(int id, RegistroUsuarioDto usuarioDto)
    {
        var usuario = await _repository.GetById(id);

        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        if (usuario.Correo != usuarioDto.Correo && await _repository.ExisteCorreo(usuarioDto.Correo))
            throw new InvalidOperationException("El correo ya está registrado");

        usuario.Nombre = usuarioDto.Nombre;
        usuario.Correo = usuarioDto.Correo;
        usuario.RolId = usuarioDto.RolId;
        usuario.Activo = usuarioDto.Activo;

        if (usuarioDto.ForzarCambioPassword != usuario.ForzarCambioPassword)
        {
            usuario.ForzarCambioPassword = usuarioDto.ForzarCambioPassword;
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
        if (usuario == null)
            throw new ArgumentNullException(nameof(usuario));

        if (usuario.Rol == null)
        {
            _logger.LogError("Usuario {Id} no tiene rol asignado", usuario.Id);
            throw new InvalidOperationException("El usuario no tiene un rol asignado");
        }

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
        new Claim(ClaimTypes.Name, usuario.Nombre),
        new Claim(ClaimTypes.Email, usuario.Correo),
        new Claim(ClaimTypes.Role, usuario.Rol.Nombre ?? "Sin Rol")
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

    public async Task CambiarPasswordPrimerLogin(int userId, string nuevaPassword)
    {
        var usuario = await _repository.GetById(userId);
        if (usuario == null)
            throw new InvalidOperationException("Usuario no encontrado");

        if (!_passwordGenerator.ValidarFortalezaPassword(nuevaPassword))
            throw new InvalidOperationException(
                "La contraseña no cumple con los requisitos de seguridad: " +
                "mínimo 8 caracteres, mayúsculas, minúsculas, números y caracteres especiales");

        usuario.Password = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);
        usuario.ForzarCambioPassword = false;
        usuario.UltimoLogin = DateTime.UtcNow;

        await _repository.Update(usuario);

        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.EnviarNotificacionCambioPassword(usuario.Correo, usuario.Nombre);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar notificación de cambio de contraseña");
            }
        });

        _logger.LogInformation("Usuario {Correo} cambió su contraseña exitosamente", usuario.Correo);
    }
}