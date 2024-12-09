using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkiServiceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddNameFieldsToBenutzer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Benutzer",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<string>(
                name: "Nachname",
                table: "Benutzer",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Vorname",
                table: "Benutzer",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nachname",
                table: "Benutzer");

            migrationBuilder.DropColumn(
                name: "Vorname",
                table: "Benutzer");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Benutzer",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
