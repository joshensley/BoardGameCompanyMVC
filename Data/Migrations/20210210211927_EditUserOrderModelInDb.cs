using Microsoft.EntityFrameworkCore.Migrations;

namespace BoardGameCompanyMVC.Data.Migrations
{
    public partial class EditUserOrderModelInDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOrders_UserCheckOutInputs_UserCheckOutInputID1",
                table: "UserOrders");

            migrationBuilder.DropIndex(
                name: "IX_UserOrders_UserCheckOutInputID1",
                table: "UserOrders");

            migrationBuilder.DropColumn(
                name: "UserCheckOutInputID",
                table: "UserOrders");

            migrationBuilder.DropColumn(
                name: "UserCheckOutInputID1",
                table: "UserOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserCheckOutInputID",
                table: "UserOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserCheckOutInputID1",
                table: "UserOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserOrders_UserCheckOutInputID1",
                table: "UserOrders",
                column: "UserCheckOutInputID1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrders_UserCheckOutInputs_UserCheckOutInputID1",
                table: "UserOrders",
                column: "UserCheckOutInputID1",
                principalTable: "UserCheckOutInputs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
