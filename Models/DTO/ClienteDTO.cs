using System.ComponentModel.DataAnnotations;

namespace ProyectoGraduación.DTOs;

public class ClienteDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string TipoCliente { get; set; } = "Cliente";
    public string Nombre { get; set; } = string.Empty;
    public string? NombreExtranjero { get; set; }
    public string? Grupo { get; set; }
    public string Moneda { get; set; } = "GTQ";
    public string? Nit { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono1 { get; set; }
    public string? Telefono2 { get; set; }
    public string? TelefonoMovil { get; set; }
    public string? Fax { get; set; }
    public string? CorreoElectronico { get; set; }
    public string? SitioWeb { get; set; }
    public string? Posicion { get; set; }
    public string? Titulo { get; set; }
    public string? SegundoNombre { get; set; }
    public string? Apellido { get; set; }
    public decimal SaldoCuenta { get; set; }
    public decimal LimiteCredito { get; set; }
    public int DiasCredito { get; set; }
    public decimal DescuentoPorcentaje { get; set; }
    public bool Activo { get; set; }
    public bool BloquearMarketing { get; set; }
    public string? Observaciones1 { get; set; }
    public string? Observaciones2 { get; set; }
    public string? ClaveAcceso { get; set; }
    public string? CiudadNacimiento { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public int? CreadoPor { get; set; }
    public int? ActualizadoPor { get; set; }
}

public class CrearClienteDto
{
    // NO incluir Codigo - se genera automáticamente (CLI-0001, CLI-0002, etc.)

    [Required(ErrorMessage = "El tipo de cliente es requerido")]
    [StringLength(20, ErrorMessage = "El tipo de cliente no puede exceder 20 caracteres")]
    public string TipoCliente { get; set; } = "Cliente";

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El nombre extranjero no puede exceder 100 caracteres")]
    public string? NombreExtranjero { get; set; }

    [StringLength(50, ErrorMessage = "El grupo no puede exceder 50 caracteres")]
    public string? Grupo { get; set; }

    [Required(ErrorMessage = "La moneda es requerida")]
    [StringLength(10, ErrorMessage = "La moneda no puede exceder 10 caracteres")]
    public string Moneda { get; set; } = "GTQ";

    [StringLength(20, ErrorMessage = "El NIT no puede exceder 20 caracteres")]
    public string? Nit { get; set; }

    public string? Direccion { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono 1 no puede exceder 20 caracteres")]
    public string? Telefono1 { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono 2 no puede exceder 20 caracteres")]
    public string? Telefono2 { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono móvil no puede exceder 20 caracteres")]
    public string? TelefonoMovil { get; set; }

    [StringLength(20, ErrorMessage = "El fax no puede exceder 20 caracteres")]
    public string? Fax { get; set; }

    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder 100 caracteres")]
    public string? CorreoElectronico { get; set; }

    [StringLength(100, ErrorMessage = "El sitio web no puede exceder 100 caracteres")]
    public string? SitioWeb { get; set; }

    [StringLength(100, ErrorMessage = "La posición no puede exceder 100 caracteres")]
    public string? Posicion { get; set; }

    [StringLength(50, ErrorMessage = "El título no puede exceder 50 caracteres")]
    public string? Titulo { get; set; }

    [StringLength(100, ErrorMessage = "El segundo nombre no puede exceder 100 caracteres")]
    public string? SegundoNombre { get; set; }

    [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
    public string? Apellido { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El límite de crédito debe ser mayor o igual a 0")]
    public decimal LimiteCredito { get; set; } = 0;

    [Range(0, int.MaxValue, ErrorMessage = "Los días de crédito deben ser mayor o igual a 0")]
    public int DiasCredito { get; set; } = 0;

    [Range(0, 100, ErrorMessage = "El porcentaje de descuento debe estar entre 0 y 100")]
    public decimal DescuentoPorcentaje { get; set; } = 0;

    public string? Observaciones1 { get; set; }
    public string? Observaciones2 { get; set; }

    [StringLength(50, ErrorMessage = "La clave de acceso no puede exceder 50 caracteres")]
    public string? ClaveAcceso { get; set; }

    [StringLength(100, ErrorMessage = "La ciudad de nacimiento no puede exceder 100 caracteres")]
    public string? CiudadNacimiento { get; set; }
}

public class ActualizarClienteDto
{
    // NO permitir cambiar código

    [Required(ErrorMessage = "El tipo de cliente es requerido")]
    [StringLength(20, ErrorMessage = "El tipo de cliente no puede exceder 20 caracteres")]
    public string TipoCliente { get; set; } = "Cliente";

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El nombre extranjero no puede exceder 100 caracteres")]
    public string? NombreExtranjero { get; set; }

    [StringLength(50, ErrorMessage = "El grupo no puede exceder 50 caracteres")]
    public string? Grupo { get; set; }

    [Required(ErrorMessage = "La moneda es requerida")]
    [StringLength(10, ErrorMessage = "La moneda no puede exceder 10 caracteres")]
    public string Moneda { get; set; } = "GTQ";

    [StringLength(20, ErrorMessage = "El NIT no puede exceder 20 caracteres")]
    public string? Nit { get; set; }

    public string? Direccion { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono 1 no puede exceder 20 caracteres")]
    public string? Telefono1 { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono 2 no puede exceder 20 caracteres")]
    public string? Telefono2 { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono móvil no puede exceder 20 caracteres")]
    public string? TelefonoMovil { get; set; }

    [StringLength(20, ErrorMessage = "El fax no puede exceder 20 caracteres")]
    public string? Fax { get; set; }

    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder 100 caracteres")]
    public string? CorreoElectronico { get; set; }

    [StringLength(100, ErrorMessage = "El sitio web no puede exceder 100 caracteres")]
    public string? SitioWeb { get; set; }

    [StringLength(100, ErrorMessage = "La posición no puede exceder 100 caracteres")]
    public string? Posicion { get; set; }

    [StringLength(50, ErrorMessage = "El título no puede exceder 50 caracteres")]
    public string? Titulo { get; set; }

    [StringLength(100, ErrorMessage = "El segundo nombre no puede exceder 100 caracteres")]
    public string? SegundoNombre { get; set; }

    [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
    public string? Apellido { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El límite de crédito debe ser mayor o igual a 0")]
    public decimal LimiteCredito { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Los días de crédito deben ser mayor o igual a 0")]
    public int DiasCredito { get; set; }

    [Range(0, 100, ErrorMessage = "El porcentaje de descuento debe estar entre 0 y 100")]
    public decimal DescuentoPorcentaje { get; set; }

    public bool Activo { get; set; }
    public bool BloquearMarketing { get; set; }
    public string? Observaciones1 { get; set; }
    public string? Observaciones2 { get; set; }

    [StringLength(50, ErrorMessage = "La clave de acceso no puede exceder 50 caracteres")]
    public string? ClaveAcceso { get; set; }

    [StringLength(100, ErrorMessage = "La ciudad de nacimiento no puede exceder 100 caracteres")]
    public string? CiudadNacimiento { get; set; }
}