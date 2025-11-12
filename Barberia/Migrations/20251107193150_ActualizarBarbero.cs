using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barberia.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarBarbero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AñosExperiencia",
                table: "Barberos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Especialidad",
                table: "Barberos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AñosExperiencia",
                table: "Barberos");

            migrationBuilder.DropColumn(
                name: "Especialidad",
                table: "Barberos");
        }
    }
}
