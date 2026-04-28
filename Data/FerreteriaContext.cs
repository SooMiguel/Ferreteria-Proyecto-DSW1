using Proyecto_Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Proyecto_Ferreteria.Data
{
    public class FerreteriaContext : DbContext
    {
        public FerreteriaContext(DbContextOptions<FerreteriaContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VentasViewModel>().HasNoKey();
        }

    }
}