using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Models;

namespace ProyectoGraduación.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Constructor para migraciones
    public AppDbContext() { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Permiso> Permisos { get; set; }
    public DbSet<RolPermiso> RolesPermisos { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Cliente> Clientes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurar relaciones y tablas
        modelBuilder.Entity<RolPermiso>()
            .HasKey(rp => new { rp.RolId, rp.PermisoId });

        modelBuilder.Entity<RolPermiso>()
            .HasOne(rp => rp.Rol)
            .WithMany(r => r.RolesPermisos)
            .HasForeignKey(rp => rp.RolId);

        modelBuilder.Entity<RolPermiso>()
            .HasOne(rp => rp.Permiso)
            .WithMany(p => p.RolesPermisos)
            .HasForeignKey(rp => rp.PermisoId);

        // Configurando tablas con nombres en español
        modelBuilder.Entity<Rol>().ToTable("rol");
        modelBuilder.Entity<Usuario>().ToTable("usuario");
        modelBuilder.Entity<Permiso>().ToTable("permiso");
        modelBuilder.Entity<RolPermiso>().ToTable("rol_permiso");
        modelBuilder.Entity<Menu>().ToTable("menu");
        modelBuilder.Entity<Producto>().ToTable("producto");

        // Mapeos para Usuario
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Id)
            .HasColumnName("id_usuario");

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Nombre)
            .HasColumnName("nombre");

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Correo)
            .HasColumnName("correo");

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Password)
            .HasColumnName("password");

        modelBuilder.Entity<Usuario>()
            .Property(u => u.RolId)
            .HasColumnName("id_rol");

        // Mapeos para Rol
        modelBuilder.Entity<Rol>()
            .Property(r => r.Id)
            .HasColumnName("id_rol");

        modelBuilder.Entity<Rol>()
            .Property(r => r.Nombre)
            .HasColumnName("nombre");

        // Mapeos para Permiso
        modelBuilder.Entity<Permiso>()
            .Property(p => p.Id)
            .HasColumnName("id_permiso");

        modelBuilder.Entity<Permiso>()
            .Property(p => p.Nombre)
            .HasColumnName("nombre");

        modelBuilder.Entity<Permiso>()
            .Property(p => p.Descripcion)
            .HasColumnName("descripcion");

        // Mapeos para RolPermiso
        modelBuilder.Entity<RolPermiso>()
            .Property(rp => rp.RolId)
            .HasColumnName("id_rol");

        modelBuilder.Entity<RolPermiso>()
            .Property(rp => rp.PermisoId)
            .HasColumnName("id_permiso");

        // Mapeos para Menu
        modelBuilder.Entity<Menu>()
            .Property(m => m.Id)
            .HasColumnName("id_menu");

        modelBuilder.Entity<Menu>()
            .Property(m => m.Titulo)
            .HasColumnName("titulo");

        modelBuilder.Entity<Menu>()
            .Property(m => m.Ruta)
            .HasColumnName("ruta")
            .IsRequired(false);

        modelBuilder.Entity<Menu>()
            .Property(m => m.Icono)
            .HasColumnName("icono")
            .IsRequired(false);

        modelBuilder.Entity<Menu>()
            .Property(m => m.MenuPadreId)
            .HasColumnName("id_menu_padre");

        modelBuilder.Entity<Menu>()
            .Property(m => m.Orden)
            .HasColumnName("orden");

        modelBuilder.Entity<Menu>()
            .Property(m => m.Activo)
            .HasColumnName("activo");

        modelBuilder.Entity<Menu>()
            .Property(m => m.PermisoId)
            .HasColumnName("id_permiso");

        // Mapeos para Producto  
        modelBuilder.Entity<Producto>()
            .Property(p => p.Id)
            .HasColumnName("id_producto");

        modelBuilder.Entity<Producto>()
            .Property(p => p.Nombre)
            .HasColumnName("nombre");

        modelBuilder.Entity<Producto>()
            .Property(p => p.Precio)
            .HasColumnName("precio");

        // Configurar tabla Cliente
        modelBuilder.Entity<Cliente>().ToTable("cliente");

        // Mapeos para Cliente
        modelBuilder.Entity<Cliente>()
            .Property(c => c.Id)
            .HasColumnName("id_cliente");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Codigo)
            .HasColumnName("codigo");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.TipoCliente)
            .HasColumnName("tipo_cliente");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Nombre)
            .HasColumnName("nombre");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.NombreExtranjero)
            .HasColumnName("nombre_extranjero");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Grupo)
            .HasColumnName("grupo");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Moneda)
            .HasColumnName("moneda");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Nit)
            .HasColumnName("nit");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Direccion)
            .HasColumnName("direccion");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Telefono1)
            .HasColumnName("telefono_1");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Telefono2)
            .HasColumnName("telefono_2");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.TelefonoMovil)
            .HasColumnName("telefono_movil");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Fax)
            .HasColumnName("fax");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.CorreoElectronico)
            .HasColumnName("correo_electronico");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.SitioWeb)
            .HasColumnName("sitio_web");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Posicion)
            .HasColumnName("posicion");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Titulo)
            .HasColumnName("titulo");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.SegundoNombre)
            .HasColumnName("segundo_nombre");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Apellido)
            .HasColumnName("apellido");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.SaldoCuenta)
            .HasColumnName("saldo_cuenta");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.LimiteCredito)
            .HasColumnName("limite_credito");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.DiasCredito)
            .HasColumnName("dias_credito");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.DescuentoPorcentaje)
            .HasColumnName("descuento_porcentaje");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Activo)
            .HasColumnName("activo");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.BloquearMarketing)
            .HasColumnName("bloquear_marketing");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Observaciones1)
            .HasColumnName("observaciones_1");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Observaciones2)
            .HasColumnName("observaciones_2");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.ClaveAcceso)
            .HasColumnName("clave_acceso");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.CiudadNacimiento)
            .HasColumnName("ciudad_nacimiento");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.FechaCreacion)
            .HasColumnName("fecha_creacion");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.FechaActualizacion)
            .HasColumnName("fecha_actualizacion");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.CreadoPor)
            .HasColumnName("creado_por");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.ActualizadoPor)
            .HasColumnName("actualizado_por");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Conexión por defecto para migraciones
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=sistema_inventario;Username=postgres;Password=1234");
        }
    }
}