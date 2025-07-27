using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tryout.Models;

namespace Tryout.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Description = "Action", Name = "Bond"},
                new Category { Id = 2, Description = "Action", Name = "Fast" },
                new Category { Id = 3, Description = "Animation", Name = "Shrek" }


                );
        }
    }
}
