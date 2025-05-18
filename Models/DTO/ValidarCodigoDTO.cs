using System.ComponentModel.DataAnnotations;

namespace ProyectoGraduación.DTOs;

public class ValidarCodigoDto
{
    [Required(ErrorMessage = "El código es obligatorio")]
    [StringLength(20, ErrorMessage = "El código no puede exceder 20 caracteres")]
    public string Codigo { get; set; } = string.Empty;
}