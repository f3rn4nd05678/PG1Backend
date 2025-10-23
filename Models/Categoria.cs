
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoGraduación.Models;

[Table("categoria")]
public class Categoria
{
    [Key]
    [Column("id_categoria")]
    public int Id { get; set; }

    [Required]
    [Column("nombre")]
    [MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [Column("codigo_prefijo")]
    [MaxLength(10)]
    public string CodigoPrefijo { get; set; } = string.Empty;

    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("activo")]
    public bool Activo { get; set; } = true;

    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}