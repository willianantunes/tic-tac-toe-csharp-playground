using Microsoft.EntityFrameworkCore;
using TicTacToeCSharpPlayground.Core.Models;

namespace TicTacToeCSharpPlayground.Infrastructure.Database.FluentSetup
{
    public static class FluentPlayer
    {
        public static void Setup(ModelBuilder modelBuilder)
        {
            // Constraints
            modelBuilder.Entity<Player>(entity =>
            {
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(p => p.Name).IsUnique();
            });
        }
    }
}
