using System.ComponentModel.DataAnnotations;

namespace ProyectoGraduación.DTOs;

public class ValidarNitDto
{
    [Required(ErrorMessage = "El NIT es obligatorio")]
    [StringLength(20, ErrorMessage = "El NIT no puede exceder 20 caracteres")]
    public string Nit { get; set; } = string.Empty;
}