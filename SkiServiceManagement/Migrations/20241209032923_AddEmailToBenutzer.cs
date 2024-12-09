using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkiServiceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToBenutzer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Benutzer",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Benutzer");
        }
    }
}
