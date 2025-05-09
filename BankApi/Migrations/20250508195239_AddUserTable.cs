using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CNP = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsModerator = table.Column<bool>(type: "bit", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsHidden = table.Column<bool>(type: "bit", nullable: false),
                    GemBalance = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    NumberOfOffenses = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RiskScore = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ROI = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    CreditScore = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Birthday = table.Column<DateOnly>(type: "date", nullable: false),
                    ZodiacSign = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ZodiacAttribute = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumberOfBillSharesPaid = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Income = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CNP",
                table: "Users",
                column: "CNP",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
