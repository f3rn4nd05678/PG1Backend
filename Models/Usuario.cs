namespace ProyectoGraduación.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Correo { get; set; }
    public string Password { get; set; }
    public int RolId { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime? UltimoLogin { get; set; }
    public bool ForzarCambioPassword { get; set; } = false;


    public Rol Rol { get; set; }
}