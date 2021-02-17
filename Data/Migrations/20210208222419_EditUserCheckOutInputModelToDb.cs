using Microsoft.EntityFrameworkCore.Migrations;

namespace BoardGameCompanyMVC.Data.Migrations
{
    public partial class EditUserCheckOutInputModelToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CityBillingAddress",
                table: "UserCheckOutInputs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CityShippingAddress",
                table: "UserCheckOutInputs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityBillingAddress",
                table: "UserCheckOutInputs");

            migrationBuilder.DropColumn(
                name: "CityShippingAddress",
                table: "UserCheckOutInputs");
        }
    }
}
