using System.ComponentModel.DataAnnotations;

namespace ProyectoGraduación.DTOs;

public class ObtenerClientePorIdDto
{
    [Required(ErrorMessage = "El ID es obligatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID debe ser mayor a 0")]
    public int Id { get; set; }
}