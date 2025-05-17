namespace ProyectoGraduación.Models;

public class Permiso
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }

    // Navegación
    public ICollection<RolPermiso> RolesPermisos { get; set; }
}