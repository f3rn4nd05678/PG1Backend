namespace ProyectoGraduación.DTOs;

public class MenuDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string? Ruta { get; set; }
    public string? Icono { get; set; }
    public int? MenuPadreId { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; }
    public List<MenuDto> Items { get; set; } = new List<MenuDto>();
}