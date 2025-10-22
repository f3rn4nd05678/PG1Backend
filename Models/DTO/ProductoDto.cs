using System.ComponentModel.DataAnnotations;

namespace ProyectoGraduación.DTOs;

// DTO para listar productos
public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public decimal Precio { get; set; }
    public int StockMinimo { get; set; }
    public int? ProveedorId { get; set; }
    public string? ProveedorNombre { get; set; }
}

// DTO para crear producto
public class CrearProductoDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código es requerido")]
    [StringLength(50, ErrorMessage = "El código no puede exceder 50 caracteres")]
    public string Codigo { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "La categoría no puede exceder 50 caracteres")]
    public string? Categoria { get; set; }

    [Required(ErrorMessage = "El precio es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "El stock mínimo es requerido")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo debe ser mayor o igual a 0")]
    public int StockMinimo { get; set; }

    public int? ProveedorId { get; set; }
}

// DTO para actualizar producto
public class ActualizarProductoDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "La categoría no puede exceder 50 caracteres")]
    public string? Categoria { get; set; }

    [Required(ErrorMessage = "El precio es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "El stock mínimo es requerido")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo debe ser mayor o igual a 0")]
    public int StockMinimo { get; set; }

    public int? ProveedorId { get; set; }
}

// DTO para filtros de búsqueda
public class FiltroProductoDto
{
    public string? TerminoBusqueda { get; set; }
    public string? Categoria { get; set; }
    public int? ProveedorId { get; set; }
    public decimal? PrecioMinimo { get; set; }
    public decimal? PrecioMaximo { get; set; }
    public int NumeroPagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 10;
}
