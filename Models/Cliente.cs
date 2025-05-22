namespace ProyectoGraduación.Models;

public class Cliente
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

    // Información de contacto
    public string? Telefono1 { get; set; }
    public string? Telefono2 { get; set; }
    public string? TelefonoMovil { get; set; }
    public string? Fax { get; set; }
    public string? CorreoElectronico { get; set; }
    public string? SitioWeb { get; set; }

    // Información personal/contacto
    public string? Posicion { get; set; }
    public string? Titulo { get; set; }
    public string? SegundoNombre { get; set; }
    public string? Apellido { get; set; }

    // Campos financieros
    public decimal SaldoCuenta { get; set; } = 0.00m;
    public decimal LimiteCredito { get; set; } = 0.00m;
    public int DiasCredito { get; set; } = 0;
    public decimal DescuentoPorcentaje { get; set; } = 0.00m;

    // Configuraciones
    public bool Activo { get; set; } = true;
    public bool BloquearMarketing { get; set; } = false;
    public string? Observaciones1 { get; set; }
    public string? Observaciones2 { get; set; }
    public string? ClaveAcceso { get; set; }
    public string? CiudadNacimiento { get; set; }

    // Auditoría
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public int? CreadoPor { get; set; }
    public int? ActualizadoPor { get; set; }

    // El error indica que las propiedades de navegación no coinciden con las columnas de la BD
    // Comentamos estas propiedades para evitar el error 
    // public Usuario? UsuarioCreador { get; set; }
    // public Usuario? UsuarioActualizador { get; set; }
}