using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicTacToeCSharpPlayground.Core.Models;
using TicTacToeCSharpPlayground.Infrastructure.Database.FluentSetup;

namespace TicTacToeCSharpPlayground.Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Movement> Movements { get; set; }
        public DbSet<PlayerBoard> PlayerBoards { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Bodyless constructor
        }

        public static AppDbContext CreateContext(string connectionString, DbContextOptionsBuilder<AppDbContext>? optionsBuilder = null)
        {
            if (optionsBuilder is null)
                optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseNpgsql(connectionString);
            var options = optionsBuilder.Options;

            return new AppDbContext(options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // More details about the EF Fluent API in the following links:
            // https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/fluent/relationships#configuring-a-many-to-many-relationship
            // https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/fluent/types-and-properties
            FluentPlayer.Setup(modelBuilder);
            FluentBoard.Setup(modelBuilder);
            FluentPlayerBoard.Setup(modelBuilder);
            FluentMovement.Setup(modelBuilder);
        }

        public override int SaveChanges()
        {
            AutomaticallyAddCreatedAndUpdatedAt();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AutomaticallyAddCreatedAndUpdatedAt();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AutomaticallyAddCreatedAndUpdatedAt()
        {
            var entitiesOnDbContext = ChangeTracker.Entries<StandardEntity>();

            if (entitiesOnDbContext is null)
                return;

            foreach (var item in entitiesOnDbContext.Where(t => t.State == EntityState.Added))
            {
                item.Entity.CreatedAt = DateTime.Now.ToUniversalTime();
                item.Entity.UpdatedAt = DateTime.Now.ToUniversalTime();
            }

            foreach (var item in entitiesOnDbContext.Where(t => t.State == EntityState.Modified))
            {
                item.Entity.UpdatedAt = DateTime.Now.ToUniversalTime();
            }
        }
    }
}
