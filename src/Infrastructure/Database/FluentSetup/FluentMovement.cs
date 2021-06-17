using Microsoft.EntityFrameworkCore;
using TicTacToeCSharpPlayground.Core.Models;

namespace TicTacToeCSharpPlayground.Infrastructure.Database.FluentSetup
{
    public static class FluentMovement
    {
        public static void Setup(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movement>(entity =>
            {
                // You can't do repeated movements on the same board
                entity.HasIndex(m => new { m.BoardId, m.Position }).IsUnique();
            });
        }
    }
}
