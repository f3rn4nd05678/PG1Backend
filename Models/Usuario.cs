namespace ProyectoGraduación.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Correo { get; set; }
    public string Password { get; set; }
    public int RolId { get; set; }


    public Rol Rol { get; set; }
}