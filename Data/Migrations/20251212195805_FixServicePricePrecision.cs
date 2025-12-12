using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixServicePricePrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Service");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Service",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Service",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
