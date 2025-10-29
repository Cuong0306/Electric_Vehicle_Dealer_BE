using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectricVehicleDealer.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStafftoStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_staff_store_StoreId",
                table: "staff");

            migrationBuilder.DropIndex(
                name: "IX_staff_StoreId",
                table: "staff");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "staff");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "staff",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_staff_StoreId",
                table: "staff",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_staff_store_StoreId",
                table: "staff",
                column: "StoreId",
                principalTable: "store",
                principalColumn: "store_id");
        }
    }
}
