using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tryout.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductModelForPerfume : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price50",
                table: "Products",
                newName: "Price6ml");

            migrationBuilder.RenameColumn(
                name: "Price100",
                table: "Products",
                newName: "Price20ml");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "Price15ml");

            migrationBuilder.RenameColumn(
                name: "ListPrice",
                table: "Products",
                newName: "Price10ml");

            migrationBuilder.RenameColumn(
                name: "IMDBId",
                table: "Products",
                newName: "SKU");

            migrationBuilder.RenameColumn(
                name: "Director",
                table: "Products",
                newName: "InspirationBrand");

            migrationBuilder.AddColumn<string>(
                name: "UnitType",
                table: "ShoppingCarts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Price10ml", "Price15ml", "Price6ml" },
                values: new object[] { 28.0, 38.0, 18.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Price10ml", "Price15ml", "Price6ml" },
                values: new object[] { 34.0, 46.0, 22.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Price10ml", "Price15ml", "Price6ml" },
                values: new object[] { 42.0, 58.0, 28.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Price10ml", "Price15ml", "Price6ml" },
                values: new object[] { 31.0, 42.0, 20.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Price10ml", "Price15ml", "Price6ml" },
                values: new object[] { 38.0, 52.0, 25.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Price10ml", "Price15ml", "Price6ml" },
                values: new object[] { 45.0, 62.0, 30.0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitType",
                table: "ShoppingCarts");

            migrationBuilder.RenameColumn(
                name: "SKU",
                table: "Products",
                newName: "IMDBId");

            migrationBuilder.RenameColumn(
                name: "Price6ml",
                table: "Products",
                newName: "Price50");

            migrationBuilder.RenameColumn(
                name: "Price20ml",
                table: "Products",
                newName: "Price100");

            migrationBuilder.RenameColumn(
                name: "Price15ml",
                table: "Products",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "Price10ml",
                table: "Products",
                newName: "ListPrice");

            migrationBuilder.RenameColumn(
                name: "InspirationBrand",
                table: "Products",
                newName: "Director");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ListPrice", "Price", "Price50" },
                values: new object[] { 18.0, 28.0, 38.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ListPrice", "Price", "Price50" },
                values: new object[] { 22.0, 34.0, 46.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ListPrice", "Price", "Price50" },
                values: new object[] { 28.0, 42.0, 58.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ListPrice", "Price", "Price50" },
                values: new object[] { 20.0, 31.0, 42.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ListPrice", "Price", "Price50" },
                values: new object[] { 25.0, 38.0, 52.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ListPrice", "Price", "Price50" },
                values: new object[] { 30.0, 45.0, 62.0 });
        }
    }
}
