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

        // --- MÓDULO 1: Catálogo ---
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        // --- MÓDULO 2: Seguridad ---
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        // --- MÓDULO 4: Reportes ---
        public DbSet<VentasViewModel> VentasViewModel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VentasViewModel>().HasNoKey();
        }
    }
}