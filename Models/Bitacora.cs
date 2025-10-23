using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoGraduación.Models;

[Table("bitacora")]
public class Bitacora
{
    [Key]
    [Column("id_log")]
    public int Id { get; set; }

    [Required]
    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Column("fecha")]
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    [Required]
    [Column("accion")]
    public string Accion { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("modulo")]
    public string Modulo { get; set; } = string.Empty;

    // Navegación
    [ForeignKey("IdUsuario")]
    public virtual Usuario Usuario { get; set; } = null!;
}