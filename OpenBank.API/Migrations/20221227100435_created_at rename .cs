using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenBank.API.Migrations
{
    /// <inheritdoc />
    public partial class createdatrename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "Created_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Created_at",
                table: "Users",
                newName: "CreatedAt");
        }
    }
}
