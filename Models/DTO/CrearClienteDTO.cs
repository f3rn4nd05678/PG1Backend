namespace ProyectoGraduación.DTOs;

public class CrearClienteDto
{
    public string? Codigo { get; set; } // Si no se proporciona, se genera automáticamente
    public string TipoCliente { get; set; } = "Cliente";
    public string Nombre { get; set; } = string.Empty;
    public string? NombreExtranjero { get; set; }
    public string? Grupo { get; set; }
    public string Moneda { get; set; } = "GTQ";
    public string? Nit { get; set; }
    public string? Direccion { get; set; }

    // Información de contacto
    public string? Telefono1 { get; set; }
    public string? Telefono2 { get; set; }
    public string? TelefonoMovil { get; set; }
    public string? Fax { get; set; }
    public string? CorreoElectronico { get; set; }
    public string? SitioWeb { get; set; }

    // Información personal
    public string? Posicion { get; set; }
    public string? Titulo { get; set; }
    public string? SegundoNombre { get; set; }
    public string? Apellido { get; set; }

    // Campos financieros
    public decimal LimiteCredito { get; set; } = 0.00m;
    public int DiasCredito { get; set; } = 0;
    public decimal DescuentoPorcentaje { get; set; } = 0.00m;

    // Configuraciones
    public bool BloquearMarketing { get; set; } = false;
    public string? Observaciones1 { get; set; }
    public string? Observaciones2 { get; set; }
    public string? ClaveAcceso { get; set; }
    public string? CiudadNacimiento { get; set; }
}