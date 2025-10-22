using System.ComponentModel.DataAnnotations;

namespace ProyectoGraduación.DTOs;

public class RegistroUsuarioDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo no es válido")]
    public string Correo { get; set; }

    public string? Password { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio")]
    public int RolId { get; set; }

    public bool Activo { get; set; } = true;
    public bool ForzarCambioPassword { get; set; } = false;
}