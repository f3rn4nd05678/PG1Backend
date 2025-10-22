namespace ProyectoGraduación.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Correo { get; set; }
    public int RolId { get; set; }
    public string RolNombre { get; set; }

    public bool Activo { get; set; }
    public DateTime? UltimoLogin { get; set; }
    public bool ForzarCambioPassword { get; set; }
}