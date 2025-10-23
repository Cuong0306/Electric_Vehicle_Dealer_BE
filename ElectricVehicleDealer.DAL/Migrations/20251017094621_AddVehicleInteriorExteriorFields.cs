using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectricVehicleDealer.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleInteriorExteriorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "air_conditioning",
                table: "vehicle",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cameras",
                table: "vehicle",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "door_count",
                table: "vehicle",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "frame_chassis",
                table: "vehicle",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "glass_windows",
                table: "vehicle",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "headlights",
                table: "vehicle",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "height_mm",
                table: "vehicle",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "in_vehicle_cabinet",
                table: "vehicle",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "interior_material",
                table: "vehicle",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "length_mm",
                table: "vehicle",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "mirrors",
                table: "vehicle",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "screen",
                table: "vehicle",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "seat_material",
                table: "vehicle",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "speaker_system",
                table: "vehicle",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "taillights",
                table: "vehicle",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "wheels",
                table: "vehicle",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "width_mm",
                table: "vehicle",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "air_conditioning",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "cameras",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "door_count",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "frame_chassis",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "glass_windows",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "headlights",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "height_mm",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "in_vehicle_cabinet",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "interior_material",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "length_mm",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "mirrors",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "screen",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "seat_material",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "speaker_system",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "taillights",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "wheels",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "width_mm",
                table: "vehicle");
        }
    }
}
