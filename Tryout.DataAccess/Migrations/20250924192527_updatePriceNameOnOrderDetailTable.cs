using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tryout.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updatePriceNameOnOrderDetailTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Priced",
                table: "OrderDetails",
                newName: "Price");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "OrderDetails",
                newName: "Priced");
        }
    }
}
