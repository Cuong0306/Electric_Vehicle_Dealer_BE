using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectricVehicleDealer.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AgreementAddStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "store_id",
                table: "agreement",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_agreement_store_id",
                table: "agreement",
                column: "store_id");

            migrationBuilder.AddForeignKey(
                name: "FK_agreement_store_store_id",
                table: "agreement",
                column: "store_id",
                principalTable: "store",
                principalColumn: "store_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_agreement_store_store_id",
                table: "agreement");

            migrationBuilder.DropIndex(
                name: "IX_agreement_store_id",
                table: "agreement");

            migrationBuilder.DropColumn(
                name: "store_id",
                table: "agreement");
        }
    }
}
