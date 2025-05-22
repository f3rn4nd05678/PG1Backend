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