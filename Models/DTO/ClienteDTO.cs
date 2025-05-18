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
    public string? CorreoElectronico { get; set; }
    public decimal SaldoCuenta { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}