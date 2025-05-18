using System.ComponentModel.DataAnnotations;

namespace ProyectoGraduación.DTOs;

public class BuscarClienteDto
{
    [Required(ErrorMessage = "El término de búsqueda es obligatorio")]
    [StringLength(100, ErrorMessage = "El término de búsqueda no puede exceder 100 caracteres")]
    public string TerminoBusqueda { get; set; } = string.Empty;
}