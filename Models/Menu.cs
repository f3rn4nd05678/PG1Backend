namespace ProyectoGraduación.Models;

public class Menu
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Ruta { get; set; }
    public string Icono { get; set; }
    public int? MenuPadreId { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; }
    public int PermisoId { get; set; }

    // Navegación
    public Menu MenuPadre { get; set; }
    public ICollection<Menu> MenusHijos { get; set; }
    public Permiso Permiso { get; set; }
}