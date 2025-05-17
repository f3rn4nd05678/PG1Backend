namespace ProyectoGraduación.Models;

public class RolPermiso
{
    public int RolId { get; set; }
    public int PermisoId { get; set; }

    // Navegación
    public Rol Rol { get; set; }
    public Permiso Permiso { get; set; }
}