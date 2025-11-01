using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectricVehicleDealer.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addTransactionForPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "checkout_url",
                table: "payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "transaction_id",
                table: "payment",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "checkout_url",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "transaction_id",
                table: "payment");
        }
    }
}
