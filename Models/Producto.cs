namespace ProyectoGraduación.Models;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public decimal Precio { get; set; }
    public int StockMinimo { get; set; }
    public int? ProveedorId { get; set; }

    // Relación con Proveedor
    public virtual Proveedor? Proveedor { get; set; }
}
