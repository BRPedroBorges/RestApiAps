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
            modelBuilder.Entity<Score>().ToTable("Pontuacoes");
        }
    }
}
