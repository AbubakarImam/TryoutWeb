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
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Description = "Men's Fresh/Aquatic", Name = "Aqua Di Gio Type" },
                new Category { Id = 2, Description = "Women's Floral/Sweet", Name = "La Vie Est Belle Type" },
                new Category { Id = 3, Description = "Unisex Woody/Oriental", Name = "Baccarat Rouge 540 Type" }
            );
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Ocean Breeze Elixir (6ml/10ml/15ml/20ml)",
                    InspirationBrand = "Inspired by Acqua Di Gio",
                    Description = "A clean, aquatic scent with notes of marine, bergamot, and cedar. Perfect for daily wear and warm weather.",
                    SKU = "PERFUME0001",
                    Price6ml = 18, // Price for 6ml
                    Price10ml = 28,     // Price for 10ml
                    Price15ml = 38,   // Price for 15ml
                    Price20ml = 45,  // Price for 20ml
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Title = "Jasmine & Iris Dream (6ml/10ml/15ml/20ml)",
                    InspirationBrand = "Inspired by La Vie Est Belle",
                    Description = "An elegant, gourmand floral with notes of black currant, praline, and vanilla. A timeless, sweet classic.",
                    SKU = "PERFUME0002",
                    Price6ml = 22, // Price for 6ml
                    Price10ml = 34,     // Price for 10ml
                    Price15ml = 46,   // Price for 15ml
                    Price20ml = 55,  // Price for 20ml
                    CategoryId = 2
                },
                new Product
                {
                    Id = 3,
                    Title = "Saffron Silk (6ml/10ml/15ml/20ml)",
                    InspirationBrand = "Inspired by Baccarat Rouge 540",
                    Description = "A radiant and sophisticated blend of saffron, cedarwood, and ambergris. A luxury, signature scent.",
                    SKU = "PERFUME0003",
                    Price6ml = 28, // Price for 6ml
                    Price10ml = 42,     // Price for 10ml
                    Price15ml = 58,   // Price for 15ml
                    Price20ml = 69,  // Price for 20ml
                    CategoryId = 3
                },
                new Product
                {
                    Id = 4,
                    Title = "Dark Vetiver Mystery (6ml/10ml/15ml/20ml)",
                    InspirationBrand = "Inspired by Terre d'Hermès",
                    Description = "An earthy, woody, and spicy scent with prominent notes of vetiver and patchouli. For the sophisticated person.",
                    SKU = "PERFUME0004",
                    Price6ml = 20, // Price for 6ml
                    Price10ml = 31,     // Price for 10ml
                    Price15ml = 42,   // Price for 15ml
                    Price20ml = 50,  // Price for 20ml
                    CategoryId = 1 // Men's Fresh/Aquatic (Can cross-list categories)
                },
                new Product
                {
                    Id = 5,
                    Title = "Midnight Bloom (6ml/10ml/15ml/20ml)",
                    InspirationBrand = "Inspired by Black Opium",
                    Description = "A captivating contrast of white florals and black coffee, creating an addictive, sensual, and energetic fragrance.",
                    SKU = "PERFUME0005",
                    Price6ml = 25, // Price for 6ml
                    Price10ml = 38,     // Price for 10ml
                    Price15ml = 52,   // Price for 15ml
                    Price20ml = 62,  // Price for 20ml
                    CategoryId = 2
                },
                new Product
                {
                    Id = 6,
                    Title = "Warm Spice Trail (6ml/10ml/15ml/20ml)",
                    InspirationBrand = "Inspired by Tobacco Vanille",
                    Description = "A rich, warm, and iconic oriental blend of tobacco leaf, vanilla, and spice. Intense and long-lasting.",
                    SKU = "PERFUME0006",
                    Price6ml = 30, // Price for 6ml
                    Price10ml = 45,     // Price for 10ml
                    Price15ml = 62,   // Price for 15ml
                    Price20ml = 75,  // Price for 20ml
                    CategoryId = 3
                }
                );
        }
    }
}
