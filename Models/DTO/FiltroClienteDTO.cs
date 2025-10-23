namespace ProyectoGraduación.DTOs;

public class FiltroClienteDto
{
    public string? Nombre { get; set; }
    public string? Nit { get; set; }
    public string? Codigo { get; set; }
    public int NumeroPagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 10;
}