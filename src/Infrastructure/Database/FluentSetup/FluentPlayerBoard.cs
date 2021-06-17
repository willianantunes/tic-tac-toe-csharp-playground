using Microsoft.EntityFrameworkCore;
using TicTacToeCSharpPlayground.Core.Models;

namespace TicTacToeCSharpPlayground.Infrastructure.Database.FluentSetup
{
    public static class FluentPlayerBoard
    {
        public static void Setup(ModelBuilder modelBuilder)
        {
            // Composite PK
            modelBuilder.Entity<PlayerBoard>()
                .HasKey(pb => new { pb.PlayerId, pb.BoardId });

            // One-to-many mapping with Player
            modelBuilder.Entity<PlayerBoard>()
                .HasOne(pb => pb.Player)
                .WithMany(p => p.PlayerBoards)
                .HasForeignKey(pb => pb.PlayerId);

            // One-to-many mapping with Board
            modelBuilder.Entity<PlayerBoard>()
                .HasOne(pb => pb.Board)
                .WithMany(b => b.PlayerBoards)
                .HasForeignKey(pb => pb.BoardId);

            // Constraints
            modelBuilder.Entity<PlayerBoard>(entity =>
            {
                // You can't have more than one PlayerBoard with the same Board and Player
                entity.HasIndex(pb => new { pb.BoardId, pb.PlayerId }).IsUnique();
            });
        }
    }
}
