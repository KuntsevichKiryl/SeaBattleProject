using Microsoft.EntityFrameworkCore;
using SeaBattle.Logger;
using SeaBattle.Model.Domain;

namespace SeaBattle.Repository
{
    public sealed class ApplicationDbContext : DbContext
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<DbContext>(level: LogLevel.Debug);
        public DbSet<User> Users { get; set; }
        public DbSet<Shot> Shots { get; set; }
        public DbSet<Ship> Ships { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<BattleField> BattleFields { get; set; }

        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=temp\\Seabattle.db");
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.LogTo(message => log.Debug(message));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(b => b.Login).IsUnique();
        }
    }
}
