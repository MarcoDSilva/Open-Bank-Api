using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenBank.API.Migrations
{
    /// <inheritdoc />
    public partial class renamingtable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movim_Accounts_AccountId",
                table: "Movim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Movim",
                table: "Movim");

            migrationBuilder.RenameTable(
                name: "Movim",
                newName: "Transfers");

            migrationBuilder.RenameIndex(
                name: "IX_Movim_AccountId",
                table: "Transfers",
                newName: "IX_Transfers_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transfers",
                table: "Transfers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Accounts_AccountId",
                table: "Transfers",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Accounts_AccountId",
                table: "Transfers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transfers",
                table: "Transfers");

            migrationBuilder.RenameTable(
                name: "Transfers",
                newName: "Movim");

            migrationBuilder.RenameIndex(
                name: "IX_Transfers_AccountId",
                table: "Movim",
                newName: "IX_Movim_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movim",
                table: "Movim",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movim_Accounts_AccountId",
                table: "Movim",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
