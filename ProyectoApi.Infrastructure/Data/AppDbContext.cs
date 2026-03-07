
using Microsoft.EntityFrameworkCore;
using ProyectoApi.Domain.Entities;

namespace ProyectoApi.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rol>().HasData(
                new Rol {Id = 1, Nombre = "Admin" },
                new Rol {Id = 2, Nombre = "User" }
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
