using Microsoft.EntityFrameworkCore;

namespace TicTacToeCSharpPlayground.Repository
{
    public class CSharpPlaygroundContext : DbContext
    {
        public CSharpPlaygroundContext(DbContextOptions<CSharpPlaygroundContext> options) : base(options)
        {
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

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Movement> Movements { get; set; }
    }
}
