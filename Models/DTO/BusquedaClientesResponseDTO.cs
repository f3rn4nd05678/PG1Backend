namespace ProyectoGraduación.DTOs;

public class BusquedaClientesResponseDto
{
    public IEnumerable<ClienteDto> Resultados { get; set; } = new List<ClienteDto>();
    public int Total { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public bool EsResultadoUnico => Total == 1;
}