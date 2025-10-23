using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoGraduación.Models;

[Table("stock")]
public class Stock
{
    [Key]
    [Column("id_stock")]
    public int Id { get; set; }

    [Required]
    [Column("id_producto")]
    public int IdProducto { get; set; }

    [Required]
    [Column("id_bodega")]
    public int IdBodega { get; set; }

    [Column("cantidad_actual", TypeName = "decimal(10,2)")]
    public decimal CantidadActual { get; set; } = 0;

    [Column("cantidad_minima", TypeName = "decimal(10,2)")]
    public decimal CantidadMinima { get; set; } = 0;

    [Column("cantidad_reservada", TypeName = "decimal(10,2)")]
    public decimal CantidadReservada { get; set; } = 0;

    // cantidad_disponible es GENERATED en BD, no se mapea
    [NotMapped]
    public decimal CantidadDisponible => CantidadActual - CantidadReservada;

    [Column("ultima_entrada")]
    public DateTime? UltimaEntrada { get; set; }

    [Column("ultima_salida")]
    public DateTime? UltimaSalida { get; set; }

    [Column("fecha_actualizacion")]
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    // Navegación
    [ForeignKey("IdProducto")]
    public virtual Producto Producto { get; set; } = null!;

    [ForeignKey("IdBodega")]
    public virtual Bodega Bodega { get; set; } = null!;
}