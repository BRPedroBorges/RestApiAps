using Microsoft.EntityFrameworkCore;
using RestApiAps.Models;

namespace RestApiAps.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Score> Scores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapeia a entidade Score para a tabela "Pontuacoes"
            modelBuilder.Entity<Score>().ToTable("Pontuacoes");
        }
    }
}
