using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectricVehicleDealer.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixBrandIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_staff_brand_brand_id",
                table: "staff");

            migrationBuilder.DropForeignKey(
                name: "FK_staff_store_StoreId",
                table: "staff");

            migrationBuilder.DropIndex(
                name: "IX_staff_StoreId",
                table: "staff");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "staff");

            migrationBuilder.AlterColumn<int>(
                name: "brand_id",
                table: "staff",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_staff_brand_brand_id",
                table: "staff",
                column: "brand_id",
                principalTable: "brand",
                principalColumn: "brand_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_staff_brand_brand_id",
                table: "staff");

            migrationBuilder.AlterColumn<int>(
                name: "brand_id",
                table: "staff",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

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
                name: "FK_staff_brand_brand_id",
                table: "staff",
                column: "brand_id",
                principalTable: "brand",
                principalColumn: "brand_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_staff_store_StoreId",
                table: "staff",
                column: "StoreId",
                principalTable: "store",
                principalColumn: "store_id");
        }
    }
}
