using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectricVehicleDealer.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityToQuote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "quote",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "quote");
        }
    }
}
