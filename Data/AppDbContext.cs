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

        // ¡Aquí está la solución! - Mapeo de propiedades para Usuario
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

        // Mapeo para otras entidades si es necesario
        modelBuilder.Entity<Rol>()
            .Property(r => r.Id)
            .HasColumnName("id_rol");

        modelBuilder.Entity<Rol>()
            .Property(r => r.Nombre)
            .HasColumnName("nombre");

        // Añade más mapeos según sea necesario
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