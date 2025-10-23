using ProyectoGraduación.Models;

namespace ProyectoGraduación.Repositories.IRepositories;

public interface ICategoriaRepository
{
    Task<IEnumerable<Categoria>> ObtenerTodosAsync();
    Task<Categoria?> ObtenerPorIdAsync(int id);
    Task<Categoria> CrearAsync(Categoria categoria);
    Task<bool> ActualizarAsync(Categoria categoria);
    Task<bool> EliminarAsync(int id);
    Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null);
    Task<bool> ExistePrefijoAsync(string prefijo, int? idExcluir = null);

    Task<IEnumerable<Categoria>> ObtenerActivosAsync();
    Task<Categoria?> ObtenerPorCodigoPrefijoAsync(string codigoPrefijo);
    Task<bool> ExisteCodigoPrefijo(string codigoPrefijo, int? idExcluir = null);
    Task<bool> ExisteNombre(string nombre, int? idExcluir = null);

}