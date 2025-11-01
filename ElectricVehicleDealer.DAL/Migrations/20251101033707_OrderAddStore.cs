using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectricVehicleDealer.DAL.Migrations
{
    /// <inheritdoc />
    public partial class OrderAddStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "store_id",
                table: "orders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_store_id",
                table: "orders",
                column: "store_id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_store_store_id",
                table: "orders",
                column: "store_id",
                principalTable: "store",
                principalColumn: "store_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_store_store_id",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_store_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "store_id",
                table: "orders");
        }
    }
}
