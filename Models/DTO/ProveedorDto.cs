using System.ComponentModel.DataAnnotations;

namespace ProyectoGraduación.DTOs;

// DTO para listar proveedores
public class ProveedorDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Nit { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool Activo { get; set; }
}

public class CrearProveedorDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El contacto no puede exceder 100 caracteres")]
    public string? Contacto { get; set; }

    [StringLength(20, ErrorMessage = "El NIT no puede exceder 20 caracteres")]
    public string? Nit { get; set; }

    public string? Direccion { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El email no es válido")]
    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    public string? Email { get; set; }
}

public class ActualizarProveedorDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El contacto no puede exceder 100 caracteres")]
    public string? Contacto { get; set; }

    [StringLength(20, ErrorMessage = "El NIT no puede exceder 20 caracteres")]
    public string? Nit { get; set; }

    public string? Direccion { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El email no es válido")]
    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    public string? Email { get; set; }

    public bool Activo { get; set; }
}

public class FiltroProveedorDto
{
    public string? Termino { get; set; }
    public int NumeroPagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 10;
}
