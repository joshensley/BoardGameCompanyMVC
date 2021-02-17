using Microsoft.EntityFrameworkCore.Migrations;

namespace BoardGameCompanyMVC.Data.Migrations
{
    public partial class EditApplicationUserModelInDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserAvatarFilePath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAvatarFilePath",
                table: "AspNetUsers");
        }
    }
}
