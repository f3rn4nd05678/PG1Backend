namespace ProyectoGraduación.DTOs;

public class ActualizarClienteDto : CrearClienteDto
{
    public bool Activo { get; set; } = true;
}