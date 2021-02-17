using Microsoft.EntityFrameworkCore.Migrations;

namespace BoardGameCompanyMVC.Data.Migrations
{
    public partial class AddUserCartModelToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCart",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoardGameID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCart", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserCart_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCart_BoardGames_BoardGameID",
                        column: x => x.BoardGameID,
                        principalTable: "BoardGames",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCart_ApplicationUserId",
                table: "UserCart",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCart_BoardGameID",
                table: "UserCart",
                column: "BoardGameID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCart");
        }
    }
}
