using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tryout.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class PerfumeSeedDataUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Men's Fresh/Aquatic", "Aqua Di Gio Type" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Women's Floral/Sweet", "La Vie Est Belle Type" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Unisex Woody/Oriental", "Baccarat Rouge 540 Type" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Director", "IMDBId", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[] { "A clean, aquatic scent with notes of marine, bergamot, and cedar. Perfect for daily wear and warm weather.", "Inspired by Acqua Di Gio", "PERFUME0001", 18.0, 28.0, 45.0, 38.0, "Ocean Breeze Elixir (6ml/10ml/15ml/20ml)" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Director", "IMDBId", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[] { "An elegant, gourmand floral with notes of black currant, praline, and vanilla. A timeless, sweet classic.", "Inspired by La Vie Est Belle", "PERFUME0002", 22.0, 34.0, 55.0, 46.0, "Jasmine & Iris Dream (6ml/10ml/15ml/20ml)" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "Description", "Director", "IMDBId", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[] { 3, "A radiant and sophisticated blend of saffron, cedarwood, and ambergris. A luxury, signature scent.", "Inspired by Baccarat Rouge 540", "PERFUME0003", 28.0, 42.0, 69.0, 58.0, "Saffron Silk (6ml/10ml/15ml/20ml)" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "Description", "Director", "IMDBId", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[] { 1, "An earthy, woody, and spicy scent with prominent notes of vetiver and patchouli. For the sophisticated person.", "Inspired by Terre d'Hermès", "PERFUME0004", 20.0, 31.0, 50.0, 42.0, "Dark Vetiver Mystery (6ml/10ml/15ml/20ml)" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "Description", "Director", "IMDBId", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[] { 2, "A captivating contrast of white florals and black coffee, creating an addictive, sensual, and energetic fragrance.", "Inspired by Black Opium", "PERFUME0005", 25.0, 38.0, 62.0, 52.0, "Midnight Bloom (6ml/10ml/15ml/20ml)" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "Description", "Director", "IMDBId", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[] { 3, "A rich, warm, and iconic oriental blend of tobacco leaf, vanilla, and spice. Intense and long-lasting.", "Inspired by Tobacco Vanille", "PERFUME0006", 30.0, 45.0, 75.0, 62.0, "Warm Spice Trail (6ml/10ml/15ml/20ml)" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Action", "Bond" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Action", "Fast" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Animation", "Shrek" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Director", "IMDBId", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[] { "A mind-bending journey through time and memory, where every decision changes the future. Intricately woven narrative that challenges perception.", "Christopher Nolan", "IMD12345601", 120.0, 110.0, 90.0, 100.0, "Echoes of Tomorrow" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Director", "IMDBId", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[] { "An emotional sci-fi drama exploring isolation and humanity’s survival on a distant silent planet. Rich visuals and deep character arcs.", "Patricia Jenkins", "IMD12345602", 95.0, 85.0, 70.0, 80.0, "Silent Horizon" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "Description", "Director", "IMDBId", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[] { 1, "A high-octane cyberpunk thriller set in a city where neon lights hide dark secrets. Fast-paced and visually immersive.", "Ryan Coogler", "IMD12345603", 100.0, 90.0, 75.0, 85.0, "Neon Drift" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "Description", "Director", "IMDBId", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[] { 3, "A haunting mystery thriller unraveling a ghost story in a quiet seaside town. Gripping from start to finish with stunning cinematography.", "Greta Gerwig", "IMD12345604", 80.0, 70.0, 60.0, 65.0, "Whispers in the Fog" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "Description", "Director", "IMDBId", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[] { 3, "An epic space battle saga that blends stunning VFX with a tale of resistance and legacy. Perfect for action and sci-fi lovers.", "James Cameron", "IMD12345605", 110.0, 100.0, 85.0, 95.0, "Iron Skies: Rebirth" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "Description", "Director", "IMDBId", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[] { 2, "A poetic drama that explores love, grief, and memory across generations, beautifully captured in slow, moody frames.", "Sofia Coppola", "IMD12345606", 70.0, 65.0, 55.0, 60.0, "Velvet Ashes" });
        }
    }
}
