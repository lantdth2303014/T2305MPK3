using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace T2305MPK3.Migrations
{
    /// <inheritdoc />
    public partial class AddEventDateAndContactInfoToCustOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustOrders_Customers_CustomerId",
                table: "CustOrders");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "CustOrders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "CustOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EventDate",
                table: "CustOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CustOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "CustOrders",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_CustOrders_Customers_CustomerId",
                table: "CustOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustOrders_Customers_CustomerId",
                table: "CustOrders");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "CustOrders");

            migrationBuilder.DropColumn(
                name: "EventDate",
                table: "CustOrders");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CustOrders");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "CustOrders");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "CustOrders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustOrders_Customers_CustomerId",
                table: "CustOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
