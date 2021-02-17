using Microsoft.EntityFrameworkCore.Migrations;

namespace BoardGameCompanyMVC.Data.Migrations
{
    public partial class AddInventoryItemModelToDb2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItem_BoardGames_BoardGameID",
                table: "InventoryItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryItem",
                table: "InventoryItem");

            migrationBuilder.RenameTable(
                name: "InventoryItem",
                newName: "InventoryItems");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryItem_BoardGameID",
                table: "InventoryItems",
                newName: "IX_InventoryItems_BoardGameID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryItems",
                table: "InventoryItems",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_BoardGames_BoardGameID",
                table: "InventoryItems",
                column: "BoardGameID",
                principalTable: "BoardGames",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_BoardGames_BoardGameID",
                table: "InventoryItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryItems",
                table: "InventoryItems");

            migrationBuilder.RenameTable(
                name: "InventoryItems",
                newName: "InventoryItem");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryItems_BoardGameID",
                table: "InventoryItem",
                newName: "IX_InventoryItem_BoardGameID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryItem",
                table: "InventoryItem",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItem_BoardGames_BoardGameID",
                table: "InventoryItem",
                column: "BoardGameID",
                principalTable: "BoardGames",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
