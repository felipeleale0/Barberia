using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barberia.Migrations
{
    /// <inheritdoc />
    public partial class BuilderSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Contrasena", "EsAdmin", "NombreUsuario" },
                values: new object[] { 1, "AQAAAAIAAYagAAAAEDdiIZ3zVZpI67hOLHa0tT3IjibLo7EHldK797+A0FQuTE45y1UEIQZo65UWr1ZIMQ==", true, "test" });
        }
    }
}
