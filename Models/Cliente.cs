using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoGraduación.Models;

[Table("cliente")]
public class Cliente
{
    [Key]
    [Column("id_cliente")]
    public int Id { get; set; }

    [Required]
    [Column("codigo")]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty; // Auto-generado CLI-0001

    [Column("tipo_cliente")]
    [MaxLength(20)]
    public string TipoCliente { get; set; } = "Cliente";

    [Required]
    [Column("nombre")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Column("nombre_extranjero")]
    [MaxLength(100)]
    public string? NombreExtranjero { get; set; }

    [Column("grupo")]
    [MaxLength(50)]
    public string? Grupo { get; set; }

    [Column("moneda")]
    [MaxLength(10)]
    public string Moneda { get; set; } = "GTQ";

    [Column("nit")]
    [MaxLength(20)]
    public string? Nit { get; set; }

    [Column("direccion")]
    public string? Direccion { get; set; }

    [Column("telefono_1")]
    [MaxLength(20)]
    public string? Telefono1 { get; set; }

    [Column("telefono_2")]
    [MaxLength(20)]
    public string? Telefono2 { get; set; }

    [Column("telefono_movil")]
    [MaxLength(20)]
    public string? TelefonoMovil { get; set; }

    [Column("fax")]
    [MaxLength(20)]
    public string? Fax { get; set; }

    [Column("correo_electronico")]
    [MaxLength(100)]
    public string? CorreoElectronico { get; set; }

    [Column("sitio_web")]
    [MaxLength(100)]
    public string? SitioWeb { get; set; }

    [Column("posicion")]
    [MaxLength(100)]
    public string? Posicion { get; set; }

    [Column("titulo")]
    [MaxLength(50)]
    public string? Titulo { get; set; }

    [Column("segundo_nombre")]
    [MaxLength(100)]
    public string? SegundoNombre { get; set; }

    [Column("apellido")]
    [MaxLength(100)]
    public string? Apellido { get; set; }

    [Column("saldo_cuenta")]
    public decimal SaldoCuenta { get; set; } = 0;

    [Column("limite_credito")]
    public decimal LimiteCredito { get; set; } = 0;

    [Column("dias_credito")]
    public int DiasCredito { get; set; } = 0;

    [Column("descuento_porcentaje")]
    public decimal DescuentoPorcentaje { get; set; } = 0;

    [Column("activo")]
    public bool Activo { get; set; } = true;

    [Column("bloquear_marketing")]
    public bool BloquearMarketing { get; set; } = false;

    [Column("observaciones_1")]
    public string? Observaciones1 { get; set; }

    [Column("observaciones_2")]
    public string? Observaciones2 { get; set; }

    [Column("clave_acceso")]
    [MaxLength(50)]
    public string? ClaveAcceso { get; set; }

    [Column("ciudad_nacimiento")]
    [MaxLength(100)]
    public string? CiudadNacimiento { get; set; }

    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [Column("fecha_actualizacion")]
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    [Column("creado_por")]
    public int? CreadoPor { get; set; }

    [Column("actualizado_por")]
    public int? ActualizadoPor { get; set; }

    // Navegación
    [ForeignKey("CreadoPor")]
    public virtual Usuario? CreadoPorUsuario { get; set; }

    [ForeignKey("ActualizadoPor")]
    public virtual Usuario? ActualizadoPorUsuario { get; set; }
}