using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoGraduación.Models;

[Table("producto")]
public class Producto
{
    [Key]
    [Column("id_producto")]
    public int Id { get; set; }

    [Required]
    [Column("nombre")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [Column("codigo")]
    [MaxLength(50)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [Column("id_categoria")]
    public int CategoriaId { get; set; }

    [Required]
    [Column("precio", TypeName = "numeric(10,2)")]
    public decimal Precio { get; set; }

    [Required]
    [Column("stock_minimo")]
    public int StockMinimo { get; set; }

    [Column("id_proveedor")]
    public int? ProveedorId { get; set; }

    [Required, Column("activo")]
    public bool Activo { get; set; } = true;

    // Navegación
    [ForeignKey("CategoriaId")]
    public virtual Categoria? Categoria { get; set; }

    [ForeignKey("ProveedorId")]
    public virtual Proveedor? Proveedor { get; set; }
}
