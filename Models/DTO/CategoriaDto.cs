using System.ComponentModel.DataAnnotations;

namespace ProyectoGraduación.DTOs;

public class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string CodigoPrefijo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}

public class CrearCategoriaDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código prefijo es requerido")]
    [StringLength(10, ErrorMessage = "El código prefijo no puede exceder 10 caracteres")]
    [RegularExpression(@"^[A-Z]{3,5}$", ErrorMessage = "El código prefijo debe tener entre 3 y 5 letras mayúsculas")]
    public string CodigoPrefijo { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; set; }
}

public class ActualizarCategoriaDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código prefijo es requerido")]
    [StringLength(10, ErrorMessage = "El código prefijo no puede exceder 10 caracteres")]
    [RegularExpression(@"^[A-Z]{3,5}$", ErrorMessage = "El código prefijo debe tener entre 3 y 5 letras mayúsculas")]
    public string CodigoPrefijo { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; set; }

    public bool Activo { get; set; }
}