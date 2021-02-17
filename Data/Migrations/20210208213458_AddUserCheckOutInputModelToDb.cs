using Microsoft.EntityFrameworkCore.Migrations;

namespace BoardGameCompanyMVC.Data.Migrations
{
    public partial class AddUserCheckOutInputModelToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCheckOutInputs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Shipping = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FirstNameBillingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastNameBillingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressBillingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address2BillingAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryBillingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateBillingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCodeBillingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstNameShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastNameShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address2ShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCodeShippingAddress = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCheckOutInputs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserCheckOutInputs_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCheckOutInputs_ApplicationUserId",
                table: "UserCheckOutInputs",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCheckOutInputs");
        }
    }
}
