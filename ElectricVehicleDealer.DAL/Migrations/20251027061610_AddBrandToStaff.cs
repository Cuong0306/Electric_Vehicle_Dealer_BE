using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectricVehicleDealer.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandToStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "brand_id",
                table: "staff",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_staff_brand_id",
                table: "staff",
                column: "brand_id");

            migrationBuilder.AddForeignKey(
                name: "FK_staff_brand_brand_id",
                table: "staff",
                column: "brand_id",
                principalTable: "brand",
                principalColumn: "brand_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_staff_brand_brand_id",
                table: "staff");

            migrationBuilder.DropIndex(
                name: "IX_staff_brand_id",
                table: "staff");

            migrationBuilder.DropColumn(
                name: "brand_id",
                table: "staff");
        }
    }
}
