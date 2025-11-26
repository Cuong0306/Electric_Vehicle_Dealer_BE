using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectricVehicleDealer.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addRelationQuoteAndOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FinalPrice",
                table: "quote",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "quote_id",
                table: "orders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "vehicle_id",
                table: "orders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_quote_id",
                table: "orders",
                column: "quote_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_vehicle_id",
                table: "orders",
                column: "vehicle_id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_quote_quote_id",
                table: "orders",
                column: "quote_id",
                principalTable: "quote",
                principalColumn: "quote_id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_vehicle_vehicle_id",
                table: "orders",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "vehicle_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_quote_quote_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_vehicle_vehicle_id",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_quote_id",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_vehicle_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "FinalPrice",
                table: "quote");

            migrationBuilder.DropColumn(
                name: "quote_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "vehicle_id",
                table: "orders");
        }
    }
}
