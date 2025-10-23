using System.ComponentModel.DataAnnotations;

namespace ProyectoGraduación.DTOs;

/// <summary>
/// DTO para listar bodegas
/// </summary>
public class BodegaDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;  // ⬅️ AGREGAR ESTA LÍNEA
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? Responsable { get; set; }
    public string? Telefono { get; set; }
    public decimal? CapacidadM3 { get; set; }
    public bool Activa { get; set; }
    public int TotalProductos { get; set; }
    public DateTime FechaCreacion { get; set; }
}

/// <summary>
/// DTO para crear una bodega
/// </summary>
public class CrearBodegaDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
    public string? Direccion { get; set; }

    [MaxLength(100, ErrorMessage = "El responsable no puede exceder 100 caracteres")]
    public string? Responsable { get; set; }

    [MaxLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    [Phone(ErrorMessage = "Formato de teléfono inválido")]
    public string? Telefono { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "La capacidad debe ser un valor positivo")]
    public decimal? CapacidadM3 { get; set; }
    public string? Codigo { get; set; }
}

/// <summary>
/// DTO para actualizar una bodega
/// </summary>
public class ActualizarBodegaDto : CrearBodegaDto
{
    public bool Activa { get; set; } = true;
    public string? Codigo { get; set; }
}

/// <summary>
/// DTO para obtener bodega por ID
/// </summary>
public class ObtenerBodegaPorIdDto
{
    [Required(ErrorMessage = "El ID es obligatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID debe ser mayor a 0")]
    public int Id { get; set; }
}

/// <summary>
/// DTO para filtros de búsqueda y paginación
/// </summary>
public class FiltroBodegaDto
{
    public string? TerminoBusqueda { get; set; }
    public bool? SoloActivas { get; set; } = true; // Por defecto solo activas
    public int Pagina { get; set; } = 1;
    public int ElementosPorPagina { get; set; } = 10;
    public string? OrdenarPor { get; set; } = "Nombre"; // Nombre, FechaCreacion
    public bool Descendente { get; set; } = false;
}

/// <summary>
/// DTO para respuesta de listado con paginación
/// </summary>
public class ListadoBodegasResponseDto
{
    public List<BodegaDto> Bodegas { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int ElementosPorPagina { get; set; }
    public int TotalPaginas { get; set; }
}

/// <summary>
/// DTO para deshabilitar/habilitar bodega
/// </summary>
public class CambiarEstadoBodegaDto
{
    [Required(ErrorMessage = "El ID es obligatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID debe ser mayor a 0")]
    public int Id { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio")]
    public bool Activa { get; set; }
}

/// <summary>
/// DTO para alertas de stock de una bodega
/// </summary>
public class StockAlertaDto
{
    public int IdStock { get; set; }
    public string Bodega { get; set; } = string.Empty;
    public string CodigoProducto { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;
    public decimal CantidadActual { get; set; }
    public decimal CantidadMinima { get; set; }
    public decimal CantidadReservada { get; set; }
    public decimal CantidadDisponible { get; set; }
    public string NivelAlerta { get; set; } = string.Empty;
    public DateTime? UltimaEntrada { get; set; }
    public DateTime? UltimaSalida { get; set; }
}