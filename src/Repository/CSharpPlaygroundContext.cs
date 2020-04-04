using Microsoft.EntityFrameworkCore;

namespace src.Repository
{
    public class CSharpPlaygroundContext : DbContext
    {
        public CSharpPlaygroundContext(DbContextOptions<CSharpPlaygroundContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
    }
}