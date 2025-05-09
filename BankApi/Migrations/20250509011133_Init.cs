using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankApi.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserCnp = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    ActivityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastModifiedAmount = table.Column<int>(type: "int", nullable: false),
                    ActivityDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    AlertId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpperBound = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    LowerBound = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ToggleOnOff = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.AlertId);
                });

            migrationBuilder.CreateTable(
                name: "BaseStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AuthorCNP = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Price = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseStocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportedUserCnp = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ReportedMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatReports", x => x.Id);
                });



            migrationBuilder.CreateTable(
                name: "GemStores",
                columns: table => new
                {
                    Cnp = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GemBalance = table.Column<int>(type: "int", nullable: false),
                    IsGuest = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GemStores", x => x.Cnp);
                });

            migrationBuilder.CreateTable(
                name: "GivenTips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserCnp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipID = table.Column<int>(type: "int", nullable: false),
                    MessageID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GivenTips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TriggeredAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TriggeredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggeredAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomepageStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Change = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomepageStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomepageStocks_BaseStocks_Id",
                        column: x => x.Id,
                        principalTable: "BaseStocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_StockName",
                table: "Alerts",
                column: "StockName");

            migrationBuilder.CreateIndex(
                name: "IX_BaseStocks_Name",
                table: "BaseStocks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TriggeredAlerts_StockName",
                table: "TriggeredAlerts",
                column: "StockName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "ChatReports");

            migrationBuilder.DropTable(
                name: "GemStores");

            migrationBuilder.DropTable(
                name: "GivenTips");

            migrationBuilder.DropTable(
                name: "HomepageStocks");

            migrationBuilder.DropTable(
                name: "TriggeredAlerts");

            migrationBuilder.DropTable(
                name: "BaseStocks");
        }
    }
}
