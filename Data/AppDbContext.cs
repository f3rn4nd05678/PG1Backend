using Microsoft.EntityFrameworkCore;
using ProyectoGraduación.Models;
namespace ProyectoGraduación.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
}
