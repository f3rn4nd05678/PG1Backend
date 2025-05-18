namespace ProyectoGraduación.DTOs;

public class FiltroClienteDto
{
    public string? Nombre { get; set; }
    public string? Nit { get; set; }
    public string? Codigo { get; set; }
    public string? Grupo { get; set; }
    public bool? Activo { get; set; }
    public int Pagina { get; set; } = 1;
    public int ElementosPorPagina { get; set; } = 10;
}