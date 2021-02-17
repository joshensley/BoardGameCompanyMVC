using Microsoft.EntityFrameworkCore.Migrations;

namespace BoardGameCompanyMVC.Data.Migrations
{
    public partial class EditUserCheckOutInputRemovedCreditCardInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BalanceTransactionId",
                table: "UserCheckOutInputs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BalanceTransactionId",
                table: "UserCheckOutInputs");
        }
    }
}
