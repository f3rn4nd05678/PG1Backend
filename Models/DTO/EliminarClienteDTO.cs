using System.ComponentModel.DataAnnotations;

namespace ProyectoGraduación.DTOs;

public class EliminarClienteDto
{
    [Required(ErrorMessage = "El ID es obligatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID debe ser mayor a 0")]
    public int Id { get; set; }
}