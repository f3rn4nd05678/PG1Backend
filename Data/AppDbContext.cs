using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Models;

namespace ProyectoGraduación.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public AppDbContext() { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Permiso> Permisos { get; set; }
    public DbSet<RolPermiso> RolesPermisos { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Bodega> Bodegas { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Bitacora> Bitacoras { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ========================================
        // Configuración de RolPermiso (muchos a muchos)
        // ========================================
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

        // ========================================
        // Mapeo de tablas
        // ========================================
        modelBuilder.Entity<Rol>().ToTable("rol");
        modelBuilder.Entity<Usuario>().ToTable("usuario");
        modelBuilder.Entity<Permiso>().ToTable("permiso");
        modelBuilder.Entity<RolPermiso>().ToTable("rol_permiso");
        modelBuilder.Entity<Menu>().ToTable("menu");
        modelBuilder.Entity<Producto>().ToTable("producto");
        modelBuilder.Entity<Proveedor>().ToTable("proveedor");
        modelBuilder.Entity<Cliente>().ToTable("cliente");
        modelBuilder.Entity<Categoria>().ToTable("categoria");

        // ========================================
        // Configuración de Usuario
        // ========================================
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

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Activo)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        modelBuilder.Entity<Usuario>()
            .Property(u => u.UltimoLogin)
            .HasColumnName("ultimo_login")
            .IsRequired(false);

        modelBuilder.Entity<Usuario>()
            .Property(u => u.ForzarCambioPassword)
            .HasColumnName("forzar_cambio_password")
            .HasDefaultValue(false);

        // ========================================
        // Configuración de Rol
        // ========================================
        modelBuilder.Entity<Rol>()
            .Property(r => r.Id)
            .HasColumnName("id_rol");

        modelBuilder.Entity<Rol>()
            .Property(r => r.Nombre)
            .HasColumnName("nombre");

        modelBuilder.Entity<Rol>()
            .Property(r => r.Descripcion)
            .HasColumnName("descripcion")
            .HasMaxLength(255)
            .IsRequired(false);

        // ========================================
        // Configuración de Permiso
        // ========================================
        modelBuilder.Entity<Permiso>()
            .Property(p => p.Id)
            .HasColumnName("id_permiso");

        modelBuilder.Entity<Permiso>()
            .Property(p => p.Nombre)
            .HasColumnName("nombre");

        modelBuilder.Entity<Permiso>()
            .Property(p => p.Descripcion)
            .HasColumnName("descripcion");

        // ========================================
        // Configuración de RolPermiso
        // ========================================
        modelBuilder.Entity<RolPermiso>()
            .Property(rp => rp.RolId)
            .HasColumnName("id_rol");

        modelBuilder.Entity<RolPermiso>()
            .Property(rp => rp.PermisoId)
            .HasColumnName("id_permiso");

        // ========================================
        // Configuración de Menu
        // ========================================
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

        // ========================================
        // Configuración de Categoria (NUEVA)
        // ========================================
        modelBuilder.Entity<Categoria>()
            .Property(c => c.Id)
            .HasColumnName("id_categoria");

        modelBuilder.Entity<Categoria>()
            .Property(c => c.Nombre)
            .HasColumnName("nombre")
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Categoria>()
            .Property(c => c.CodigoPrefijo)
            .HasColumnName("codigo_prefijo")
            .IsRequired()
            .HasMaxLength(10);

        modelBuilder.Entity<Categoria>()
            .Property(c => c.Descripcion)
            .HasColumnName("descripcion");

        modelBuilder.Entity<Categoria>()
            .Property(c => c.Activo)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        modelBuilder.Entity<Categoria>()
            .Property(c => c.FechaCreacion)
            .HasColumnName("fecha_creacion")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Categoria>()
            .HasIndex(c => c.Nombre)
            .IsUnique();

        modelBuilder.Entity<Categoria>()
            .HasIndex(c => c.CodigoPrefijo)
            .IsUnique();

        // ========================================
        // Configuración de Producto (ACTUALIZADA)
        // ========================================
        modelBuilder.Entity<Producto>()
            .Property(p => p.Id)
            .HasColumnName("id_producto");

        modelBuilder.Entity<Producto>()
            .Property(p => p.Nombre)
            .HasColumnName("nombre")
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Producto>()
            .Property(p => p.Codigo)
            .HasColumnName("codigo")
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Producto>()
            .Property(p => p.CategoriaId)
            .HasColumnName("id_categoria")
            .IsRequired();

        modelBuilder.Entity<Producto>()
            .Property(p => p.Precio)
            .HasColumnName("precio")
            .HasColumnType("numeric(10,2)");

        modelBuilder.Entity<Producto>()
            .Property(p => p.StockMinimo)
            .HasColumnName("stock_minimo");

        modelBuilder.Entity<Producto>()
            .Property(p => p.ProveedorId)
            .HasColumnName("id_proveedor");

        modelBuilder.Entity<Producto>()
            .HasIndex(p => p.Codigo)
            .IsUnique();

        modelBuilder.Entity<Producto>().HasQueryFilter(p => p.Activo);

        // Relación Producto -> Categoria
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Producto -> Proveedor
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Proveedor)
            .WithMany(pr => pr.Productos)
            .HasForeignKey(p => p.ProveedorId)
            .OnDelete(DeleteBehavior.SetNull);

        // ========================================
        // Configuración de Cliente
        // ========================================
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
            .HasColumnName("moneda")
            .HasMaxLength(10)
            .HasDefaultValue("GTQ");

        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.Nit)
            .IsUnique()
            .HasFilter("activo = TRUE AND nit IS NOT NULL");

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
            .HasColumnName("activo")
            .HasDefaultValue(true);

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

        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.Codigo)
            .IsUnique();

        modelBuilder.Entity<Cliente>()
            .HasQueryFilter(c => c.Activo);

        // ========================================
        // Configuración de Proveedor
        // ========================================
        modelBuilder.Entity<Proveedor>()
            .Property(p => p.Id)
            .HasColumnName("id_proveedor");

        modelBuilder.Entity<Proveedor>()
            .Property(p => p.Nombre)
            .HasColumnName("nombre")
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Proveedor>()
            .Property(p => p.Contacto)
            .HasColumnName("contacto")
            .HasMaxLength(100);

        modelBuilder.Entity<Proveedor>()
            .Property(p => p.Nit)
            .HasColumnName("nit")
            .HasMaxLength(20);

        modelBuilder.Entity<Proveedor>()
            .Property(p => p.Direccion)
            .HasColumnName("direccion");

        modelBuilder.Entity<Proveedor>()
            .Property(p => p.Telefono)
            .HasColumnName("telefono")
            .HasMaxLength(20);

        modelBuilder.Entity<Proveedor>()
            .Property(p => p.Email)
            .HasColumnName("email")
            .HasMaxLength(100);

        modelBuilder.Entity<Proveedor>()
            .Property(p => p.Activo)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        modelBuilder.Entity<Bodega>(entity =>
        {
            entity.HasIndex(b => b.Nombre).IsUnique();
            entity.Property(b => b.Activa).HasDefaultValue(true);
        });

        // Configuración de Stock
        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasIndex(s => new { s.IdProducto, s.IdBodega }).IsUnique();

            entity.HasOne(s => s.Producto)
                .WithMany()
                .HasForeignKey(s => s.IdProducto)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.Bodega)
                .WithMany(b => b.Stocks)
                .HasForeignKey(s => s.IdBodega)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Bitacora
        modelBuilder.Entity<Bitacora>(entity =>
        {
            entity.HasOne(b => b.Usuario)
                .WithMany()
                .HasForeignKey(b => b.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=sistema_inventario;Username=postgres;Password=1234");
        }
    }
}