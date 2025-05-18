namespace ProyectoGraduación.DTOs;

public class ActualizarClienteRequest
{
    public int Id { get; set; }
    public ActualizarClienteDto Cliente { get; set; } = new();
}