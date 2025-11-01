using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ElectricVehicleDealer.DAL.Migrations
{
    public partial class InitStorePromotion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // 🎁 Tạo bảng promotion
            migrationBuilder.CreateTable(
                name: "promotion",
                columns: table => new
                {
                    promotion_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    discount_percent = table.Column<decimal>(type: "numeric", nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    store_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promotion", x => x.promotion_id);
                    table.ForeignKey(
                        name: "FK_promotion_store_store_id",
                        column: x => x.store_id,
                        principalTable: "store",
                        principalColumn: "store_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_promotion_store_id",
                table: "promotion",
                column: "store_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "promotion");

            migrationBuilder.DropTable(
                name: "store");
        }
    }
}
