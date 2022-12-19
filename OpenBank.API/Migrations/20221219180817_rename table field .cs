using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenBank.API.Migrations
{
    /// <inheritdoc />
    public partial class renametablefield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "Movim",
                newName: "Amount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Movim",
                newName: "Balance");
        }
    }
}
