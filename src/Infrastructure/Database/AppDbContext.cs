using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicTacToeCSharpPlayground.Core.Models;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            void CreateManyToManyBetweenPlayerAndBoard()
            {
                modelBuilder.Entity<PlayerBoard>()
                    .HasKey(t => new {t.PlayerId, t.BoardId});

                modelBuilder.Entity<PlayerBoard>()
                    .HasOne(pt => pt.Player)
                    .WithMany(p => p.PlayerBoards)
                    .HasForeignKey(pt => pt.PlayerId);

                modelBuilder.Entity<PlayerBoard>()
                    .HasOne(pt => pt.Board)
                    .WithMany(t => t.PlayerBoards)
                    .HasForeignKey(pt => pt.BoardId);
            }

            CreateManyToManyBetweenPlayerAndBoard();
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
                item.Entity.CreatedAt = DateTime.Now;
                item.Entity.UpdatedAt = DateTime.Now;
            }

            foreach (var item in entitiesOnDbContext.Where(t => t.State == EntityState.Modified))
            {
                item.Entity.UpdatedAt = DateTime.Now;
            }
        }
    }
}
