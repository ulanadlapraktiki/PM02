using lektion.Models;
using Microsoft.EntityFrameworkCore;

namespace lektion.Models
{
    public class AgroControlDbContext : DbContext
    {
        public AgroControlDbContext(DbContextOptions<AgroControlDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<User>().HasKey(u => u.user_id);
            modelBuilder.Entity<User>().HasIndex(u => u.username).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}