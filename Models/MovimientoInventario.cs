using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoGraduación.Models;

[Table("movimiento_inventario")]
public class MovimientoInventario
{
    [Key]
    [Column("id_movimiento")]
    public int Id { get; set; }

    [Required]
    [Column("id_producto")]
    public int IdProducto { get; set; }

    [Required]
    [Column("id_bodega")]
    public int IdBodega { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("tipo")]
    public string Tipo { get; set; } = string.Empty; // Entrada, Salida, Ajuste, Transferencia

    [Required]
    [Column("cantidad", TypeName = "decimal(10,2)")]
    public decimal Cantidad { get; set; }

    [Column("precio_unitario", TypeName = "decimal(10,2)")]
    public decimal? PrecioUnitario { get; set; }

    [Column("fecha")]
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    [Column("observacion")]
    public string? Observacion { get; set; }

    [Column("id_usuario")]
    public int? IdUsuario { get; set; }

    [MaxLength(50)]
    [Column("referencia")]
    public string? Referencia { get; set; }

    [MaxLength(20)]
    [Column("tipo_referencia")]
    public string? TipoReferencia { get; set; }

    // Navegación
    [ForeignKey("IdProducto")]
    public virtual Producto? Producto { get; set; }

    [ForeignKey("IdBodega")]
    public virtual Bodega? Bodega { get; set; }

    [ForeignKey("IdUsuario")]
    public virtual Usuario? Usuario { get; set; }
}