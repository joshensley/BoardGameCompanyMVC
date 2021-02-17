using Microsoft.EntityFrameworkCore.Migrations;

namespace BoardGameCompanyMVC.Data.Migrations
{
    public partial class EditBoardGameModelInDb3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserBoardGameReviewID",
                table: "BoardGames");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserBoardGameReviewID",
                table: "BoardGames",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
