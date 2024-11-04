using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace T2305MPK3.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoginMasterId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_LoginMasterId",
                table: "Customers",
                column: "LoginMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_LoginMasters_LoginMasterId",
                table: "Customers",
                column: "LoginMasterId",
                principalTable: "LoginMasters",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_LoginMasters_LoginMasterId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_LoginMasterId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LoginMasterId",
                table: "Customers");
        }
    }
}
