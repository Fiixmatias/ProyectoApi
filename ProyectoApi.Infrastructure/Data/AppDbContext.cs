
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
    }
}
