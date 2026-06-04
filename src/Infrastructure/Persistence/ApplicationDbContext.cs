using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Producto> Productos => Set<Producto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>(producto =>
            {
                producto.ToTable("productos");
                producto.HasKey(x => x.Id);
                producto.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
                producto.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
                producto.Property(x => x.Precio).HasColumnType("real");

                producto.HasIndex(x => new { x.TenantId, x.Codigo }).IsUnique();
            });
        }
    }
}
