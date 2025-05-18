namespace ProyectoGraduación.DTOs;

public class ValidacionResponseDto
{
    public bool Existe { get; set; }
    public bool Disponible { get; set; }
    public string? Mensaje { get; set; }
}