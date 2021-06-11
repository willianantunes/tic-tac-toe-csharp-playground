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
    }
}
