using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tryout.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentRefrenceToOrderHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentIntentId",
                table: "OrderHeaders",
                newName: "PaymentReference");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentReference",
                table: "OrderHeaders",
                newName: "PaymentIntentId");
        }
    }
}
