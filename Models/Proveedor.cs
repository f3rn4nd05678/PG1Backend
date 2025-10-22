namespace ProyectoGraduación.Models;

public class Proveedor
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Nit { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool Activo { get; set; } = true;

    // Relación con Productos
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
