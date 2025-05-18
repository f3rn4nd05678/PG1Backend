namespace ProyectoGraduación.DTOs;

public class ListadoClientesResponseDto
{
    public IEnumerable<ClienteDto> Clientes { get; set; } = new List<ClienteDto>();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int ElementosPorPagina { get; set; }
    public int TotalPaginas { get; set; }
}