using ProyectoGraduación.DTOs;
using ProyectoGraduación.Models;
using ProyectoGraduación.Repositories.IRepositories;
using ProyectoGraduación.Services.IServices;

namespace ProyectoGraduación.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;

    public CategoriaService(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<IEnumerable<CategoriaDto>> ObtenerTodosAsync()
    {
        var categorias = await _categoriaRepository.ObtenerTodosAsync();
        return categorias.Select(MapToDto);
    }

    public async Task<IEnumerable<CategoriaDto>> ObtenerActivosAsync()
    {
        var categorias = await _categoriaRepository.ObtenerActivosAsync();
        return categorias.Select(MapToDto);
    }

    public async Task<CategoriaDto?> ObtenerPorIdAsync(int id)
    {
        var categoria = await _categoriaRepository.ObtenerPorIdAsync(id);
        return categoria != null ? MapToDto(categoria) : null;
    }

    public async Task<CategoriaDto> CrearAsync(CrearCategoriaDto dto)
    {
        // Validar código prefijo único
        if (await _categoriaRepository.ExisteCodigoPrefijo(dto.CodigoPrefijo))
            throw new InvalidOperationException($"Ya existe una categoría con el código prefijo '{dto.CodigoPrefijo}'");

        // Validar nombre único
        if (await _categoriaRepository.ExisteNombre(dto.Nombre))
            throw new InvalidOperationException($"Ya existe una categoría con el nombre '{dto.Nombre}'");

        var categoria = new Categoria
        {
            Nombre = dto.Nombre,
            CodigoPrefijo = dto.CodigoPrefijo.ToUpper(), // Convertir a mayúsculas
            Descripcion = dto.Descripcion,
            Activo = true
        };

        var categoriaCreada = await _categoriaRepository.CrearAsync(categoria);
        return MapToDto(categoriaCreada);
    }

    public async Task<CategoriaDto> ActualizarAsync(int id, ActualizarCategoriaDto dto)
    {
        var categoria = await _categoriaRepository.ObtenerPorIdAsync(id);
        if (categoria == null)
            throw new KeyNotFoundException($"No se encontró la categoría con ID {id}");

        // Validar código prefijo único
        if (await _categoriaRepository.ExisteCodigoPrefijo(dto.CodigoPrefijo, id))
            throw new InvalidOperationException($"Ya existe otra categoría con el código prefijo '{dto.CodigoPrefijo}'");

        // Validar nombre único
        if (await _categoriaRepository.ExisteNombre(dto.Nombre, id))
            throw new InvalidOperationException($"Ya existe otra categoría con el nombre '{dto.Nombre}'");

        categoria.Nombre = dto.Nombre;
        categoria.CodigoPrefijo = dto.CodigoPrefijo.ToUpper();
        categoria.Descripcion = dto.Descripcion;
        categoria.Activo = dto.Activo;

        await _categoriaRepository.ActualizarAsync(categoria);
        return MapToDto(categoria);
    }

    public async Task EliminarAsync(int id)
    {
        var categoria = await _categoriaRepository.ObtenerPorIdAsync(id);
        if (categoria == null)
            throw new KeyNotFoundException($"No se encontró la categoría con ID {id}");

        await _categoriaRepository.EliminarAsync(id);
    }

    public async Task<bool> ExisteCodigoPrefijo(string codigoPrefijo, int? idExcluir = null)
    {
        return await _categoriaRepository.ExisteCodigoPrefijo(codigoPrefijo, idExcluir);
    }

    public async Task<bool> ExisteNombre(string nombre, int? idExcluir = null)
    {
        return await _categoriaRepository.ExisteNombre(nombre, idExcluir);
    }

    private CategoriaDto MapToDto(Categoria categoria)
    {
        return new CategoriaDto
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            CodigoPrefijo = categoria.CodigoPrefijo,
            Descripcion = categoria.Descripcion,
            Activo = categoria.Activo
        };
    }
}