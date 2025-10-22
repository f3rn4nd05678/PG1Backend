using ProyectoGraduación.Models;
using ProyectoGraduación.DTOs;
using static ProyectoGraduación.Services.UsuarioService;

namespace ProyectoGraduación.IServices;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> GetAll();
    Task<(IEnumerable<UsuarioDto> usuarios, int total)> GetWithFilters(FiltroUsuarioDto filtro);
    Task<UsuarioDto> GetById(int id);
    Task<UsuarioDto> GetByCorreo(string correo);
    Task<UsuarioDto> Registrar(RegistroUsuarioDto usuarioDto);

    Task<LoginResponseDto> Login(LoginDto loginDto);

    Task Update(int id, RegistroUsuarioDto usuarioDto);
    Task Delete(int id);
    Task<bool> ExisteCorreo(string correo);
    Task<IEnumerable<UsuarioDto>> BuscarPorRol(int rolId);
    Task CambiarPassword(int id, string nuevaPassword);
    Task ActivarDesactivar(int id, bool activo);

    Task CambiarPasswordPrimerLogin(int userId, string nuevaPassword);
}