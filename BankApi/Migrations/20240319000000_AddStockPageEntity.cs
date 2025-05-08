using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankApi.Migrations
{
    public partial class AddStockPageEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StockSymbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorCNP = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockPages_Users_AuthorCNP",
                        column: x => x.AuthorCNP,
                        principalTable: "Users",
                        principalColumn: "CNP",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockPages_Stocks_StockName",
                        column: x => x.StockName,
                        principalTable: "Stocks",
                        principalColumn: "StockName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockPages_AuthorCNP",
                table: "StockPages",
                column: "AuthorCNP");

            migrationBuilder.CreateIndex(
                name: "IX_StockPages_StockName",
                table: "StockPages",
                column: "StockName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockPages");
        }
    }
} 