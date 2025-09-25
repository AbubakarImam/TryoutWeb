using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tryout.Models;

namespace Tryout.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Description = "Action", Name = "Bond"},
                new Category { Id = 2, Description = "Action", Name = "Fast" },
                new Category { Id = 3, Description = "Animation", Name = "Shrek" }


                );
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Echoes of Tomorrow",
                    Director = "Christopher Nolan",
                    Description = "A mind-bending journey through time and memory, where every decision changes the future. Intricately woven narrative that challenges perception.",
                    IMDBId = "IMD12345601",
                    ListPrice = 120,
                    Price = 110,
                    Price50 = 100,
                    Price100 = 90,
                    CategoryId = 1,
                    ImageUrl=""
                },
new Product
{
    Id = 2,
    Title = "Silent Horizon",
    Director = "Patricia Jenkins",
    Description = "An emotional sci-fi drama exploring isolation and humanity’s survival on a distant silent planet. Rich visuals and deep character arcs.",
    IMDBId = "IMD12345602",
    ListPrice = 95,
    Price = 85,
    Price50 = 80,
    Price100 = 70,
    CategoryId = 2,
    ImageUrl=""
},
new Product
{
    Id = 3,
    Title = "Neon Drift",
    Director = "Ryan Coogler",
    Description = "A high-octane cyberpunk thriller set in a city where neon lights hide dark secrets. Fast-paced and visually immersive.",
    IMDBId = "IMD12345603",
    ListPrice = 100,
    Price = 90,
    Price50 = 85,
    Price100 = 75,
    CategoryId = 1,
    ImageUrl=""
},
new Product
{
    Id = 4,
    Title = "Whispers in the Fog",
    Director = "Greta Gerwig",
    Description = "A haunting mystery thriller unraveling a ghost story in a quiet seaside town. Gripping from start to finish with stunning cinematography.",
    IMDBId = "IMD12345604",
    ListPrice = 80,
    Price = 70,
    Price50 = 65,
    Price100 = 60,
    CategoryId = 3,
    ImageUrl=""
},
new Product
{
    Id = 5,
    Title = "Iron Skies: Rebirth",
    Director = "James Cameron",
    Description = "An epic space battle saga that blends stunning VFX with a tale of resistance and legacy. Perfect for action and sci-fi lovers.",
    IMDBId = "IMD12345605",
    ListPrice = 110,
    Price = 100,
    Price50 = 95,
    Price100 = 85,
    CategoryId = 3,
    ImageUrl=""
},
new Product
{
    Id = 6,
    Title = "Velvet Ashes",
    Director = "Sofia Coppola",
    Description = "A poetic drama that explores love, grief, and memory across generations, beautifully captured in slow, moody frames.",
    IMDBId = "IMD12345606",
    ListPrice = 70,
    Price = 65,
    Price50 = 60,
    Price100 = 55,
    CategoryId = 2,
    ImageUrl=""
}

                );
        }
    }
}
