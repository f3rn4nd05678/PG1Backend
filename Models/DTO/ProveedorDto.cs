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
    public int CantidadProductos { get; set; }
}

// DTO para crear proveedor
public class CrearProveedorDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El contacto no puede exceder 100 caracteres")]
    public string? Contacto { get; set; }

    [StringLength(20, ErrorMessage = "El NIT no puede exceder 20 caracteres")]
    public string? Nit { get; set; }

    [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
    public string? Direccion { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    [Phone(ErrorMessage = "El formato del teléfono no es válido")]
    public string? Telefono { get; set; }

    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string? Email { get; set; }
}

// DTO para actualizar proveedor
public class ActualizarProveedorDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El contacto no puede exceder 100 caracteres")]
    public string? Contacto { get; set; }

    [StringLength(20, ErrorMessage = "El NIT no puede exceder 20 caracteres")]
    public string? Nit { get; set; }

    [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
    public string? Direccion { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    [Phone(ErrorMessage = "El formato del teléfono no es válido")]
    public string? Telefono { get; set; }

    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string? Email { get; set; }

    public bool Activo { get; set; } = true;
}

// DTO para filtros de búsqueda
public class FiltroProveedorDto
{
    public string? TerminoBusqueda { get; set; }
    public bool? Activo { get; set; }
    public int NumeroPagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 10;
}
