using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tryout.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IMDBId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Director = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price50 = table.Column<double>(type: "float", nullable: false),
                    Price100 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "Director", "IMDBId", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[,]
                {
                    { 1, "A mind-bending journey through time and memory, where every decision changes the future. Intricately woven narrative that challenges perception.", "Christopher Nolan", "IMD12345601", 120.0, 110.0, 90.0, 100.0, "Echoes of Tomorrow" },
                    { 2, "An emotional sci-fi drama exploring isolation and humanity’s survival on a distant silent planet. Rich visuals and deep character arcs.", "Patricia Jenkins", "IMD12345602", 95.0, 85.0, 70.0, 80.0, "Silent Horizon" },
                    { 3, "A high-octane cyberpunk thriller set in a city where neon lights hide dark secrets. Fast-paced and visually immersive.", "Ryan Coogler", "IMD12345603", 100.0, 90.0, 75.0, 85.0, "Neon Drift" },
                    { 4, "A haunting mystery thriller unraveling a ghost story in a quiet seaside town. Gripping from start to finish with stunning cinematography.", "Greta Gerwig", "IMD12345604", 80.0, 70.0, 60.0, 65.0, "Whispers in the Fog" },
                    { 5, "An epic space battle saga that blends stunning VFX with a tale of resistance and legacy. Perfect for action and sci-fi lovers.", "James Cameron", "IMD12345605", 110.0, 100.0, 85.0, 95.0, "Iron Skies: Rebirth" },
                    { 6, "A poetic drama that explores love, grief, and memory across generations, beautifully captured in slow, moody frames.", "Sofia Coppola", "IMD12345606", 70.0, 65.0, 55.0, 60.0, "Velvet Ashes" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
