using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenBank.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Movim",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Movim_AccountId",
                table: "Movim",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movim_Accounts_AccountId",
                table: "Movim",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movim_Accounts_AccountId",
                table: "Movim");

            migrationBuilder.DropIndex(
                name: "IX_Movim_AccountId",
                table: "Movim");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Movim");
        }
    }
}
