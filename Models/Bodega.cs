using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoGraduación.Models;

[Table("bodega")]
public class Bodega
{
    [Key]
    [Column("id_bodega")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("direccion")]
    public string? Direccion { get; set; }

    [MaxLength(100)]
    [Column("responsable")]
    public string? Responsable { get; set; }

    [MaxLength(20)]
    [Column("telefono")]
    public string? Telefono { get; set; }

    [Column("capacidad_m3", TypeName = "decimal(10,2)")]
    public decimal? CapacidadM3 { get; set; }

    [Column("activa")]
    public bool Activa { get; set; } = true;

    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [Column("fecha_actualizacion")]
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}