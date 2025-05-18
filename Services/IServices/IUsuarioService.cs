using ProyectoGraduación.Models;
using ProyectoGraduación.DTOs;

namespace ProyectoGraduación.IServices;
public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> GetAll();
    Task<UsuarioDto> GetById(int id);
    Task<UsuarioDto> GetByCorreo(string correo);
    Task<UsuarioDto> Registrar(RegistroUsuarioDto usuarioDto);
    Task<string> Login(LoginDto loginDto);
    Task Update(int id, RegistroUsuarioDto usuarioDto);
    Task Delete(int id);
    Task<bool> ExisteCorreo(string correo);
    Task<IEnumerable<UsuarioDto>> BuscarPorRol(int rolId);


}
