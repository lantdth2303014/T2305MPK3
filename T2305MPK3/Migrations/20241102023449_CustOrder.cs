using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace T2305MPK3.Migrations
{
    /// <inheritdoc />
    public partial class CustOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustOrders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Restaurant_id = table.Column<long>(type: "bigint", nullable: false),
                    NoOfPeople = table.Column<int>(type: "int", nullable: false),
                    NoOfTable = table.Column<int>(type: "int", nullable: false),
                    OrderNote = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepositCost = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(15,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustOrders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_CustOrders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustOrderDetails",
                columns: table => new
                {
                    OrderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    VariantId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustOrderDetails", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_CustOrderDetails_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustOrderDetails_CustOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "CustOrders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustOrderDetails_CategoryId",
                table: "CustOrderDetails",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustOrderDetails_OrderId",
                table: "CustOrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustOrders_CustomerId",
                table: "CustOrders",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustOrderDetails");

            migrationBuilder.DropTable(
                name: "CustOrders");
        }
    }
}
