using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenBank.API.Migrations
{
    /// <inheritdoc />
    public partial class _4thmigrationtypeofmovementoperation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OperationType",
                table: "Movim",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperationType",
                table: "Movim");
        }
    }
}
