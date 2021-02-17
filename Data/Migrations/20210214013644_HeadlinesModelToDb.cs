using Microsoft.EntityFrameworkCore.Migrations;

namespace BoardGameCompanyMVC.Data.Migrations
{
    public partial class HeadlinesModelToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Headlines",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoardGameID = table.Column<int>(type: "int", nullable: false),
                    ImageFilePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Headlines", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Headlines_BoardGames_BoardGameID",
                        column: x => x.BoardGameID,
                        principalTable: "BoardGames",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Headlines_BoardGameID",
                table: "Headlines",
                column: "BoardGameID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Headlines");
        }
    }
}
