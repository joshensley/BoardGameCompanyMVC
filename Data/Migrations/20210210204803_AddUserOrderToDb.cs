using Microsoft.EntityFrameworkCore.Migrations;

namespace BoardGameCompanyMVC.Data.Migrations
{
    public partial class AddUserOrderToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserOrders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoardGameID = table.Column<int>(type: "int", nullable: false),
                    OrderNumber = table.Column<int>(type: "int", nullable: false),
                    OrderPricePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserCheckOutInputID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserCheckOutInputID1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserOrders_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOrders_BoardGames_BoardGameID",
                        column: x => x.BoardGameID,
                        principalTable: "BoardGames",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOrders_UserCheckOutInputs_UserCheckOutInputID1",
                        column: x => x.UserCheckOutInputID1,
                        principalTable: "UserCheckOutInputs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserOrders_ApplicationUserId",
                table: "UserOrders",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrders_BoardGameID",
                table: "UserOrders",
                column: "BoardGameID");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrders_UserCheckOutInputID1",
                table: "UserOrders",
                column: "UserCheckOutInputID1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserOrders");
        }
    }
}
