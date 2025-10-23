using System.ComponentModel.DataAnnotations;

namespace ProyectoGraduación.DTOs;


public class LoginDto
{
    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress(ErrorMessage = "El correo no es válido")]
    public string Correo { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    public string Password { get; set; }
}


public class LoginResponseDto
{
    public string Token { get; set; }
    public UsuarioDto Usuario { get; set; }
    public bool RequirePasswordChange { get; set; }
}


public class CambiarPasswordPrimerLoginDto
{
    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress]
    public string Correo { get; set; }

    [Required(ErrorMessage = "La contraseña actual es requerida")]
    public string PasswordActual { get; set; }

    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
    public string NuevaPassword { get; set; }

    [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
    [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmarPassword { get; set; }
}

public class SolicitarResetPasswordDto
{
    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress(ErrorMessage = "El correo no es válido")]
    public string Correo { get; set; }
}