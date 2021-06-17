using Microsoft.EntityFrameworkCore;
using TicTacToeCSharpPlayground.Core.Models;

namespace TicTacToeCSharpPlayground.Infrastructure.Database.FluentSetup
{
    public static class FluentBoard
    {
        public static void Setup(ModelBuilder modelBuilder)
        {
            // Constraints
            modelBuilder.Entity<Board>(entity =>
            {
                entity.Property(b => b.NumberOfColumn).IsRequired().HasMaxLength(1);
                entity.Property(p => p.NumberOfRows).IsRequired().HasMaxLength(1);
            });
        }
    }
}
