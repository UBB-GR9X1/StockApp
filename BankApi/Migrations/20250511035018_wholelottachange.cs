using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankApi.Migrations
{
    /// <inheritdoc />
    public partial class wholelottachange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewsArticleArticleId",
                table: "BaseStocks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FavoriteStocks",
                columns: table => new
                {
                    UserCNP = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    StockName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteStocks", x => new { x.UserCNP, x.StockName });
                    table.ForeignKey(
                        name: "FK_FavoriteStocks_BaseStocks_StockName",
                        column: x => x.StockName,
                        principalTable: "BaseStocks",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteStocks_Users_UserCNP",
                        column: x => x.UserCNP,
                        principalTable: "Users",
                        principalColumn: "CNP",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsArticles",
                columns: table => new
                {
                    ArticleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IsWatchlistRelated = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AuthorCNP = table.Column<string>(type: "nvarchar(13)", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsArticles", x => x.ArticleId);
                    table.ForeignKey(
                        name: "FK_NewsArticles_Users_AuthorCNP",
                        column: x => x.AuthorCNP,
                        principalTable: "Users",
                        principalColumn: "CNP",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<int>(type: "int", precision: 18, scale: 4, nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockValues_BaseStocks_StockName",
                        column: x => x.StockName,
                        principalTable: "BaseStocks",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseStocks_NewsArticleArticleId",
                table: "BaseStocks",
                column: "NewsArticleArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteStocks_StockName",
                table: "FavoriteStocks",
                column: "StockName");

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticles_AuthorCNP",
                table: "NewsArticles",
                column: "AuthorCNP");

            migrationBuilder.CreateIndex(
                name: "IX_StockValues_StockName",
                table: "StockValues",
                column: "StockName");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseStocks_NewsArticles_NewsArticleArticleId",
                table: "BaseStocks",
                column: "NewsArticleArticleId",
                principalTable: "NewsArticles",
                principalColumn: "ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStocks_Users_UserCnp",
                table: "UserStocks",
                column: "UserCnp",
                principalTable: "Users",
                principalColumn: "CNP",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseStocks_NewsArticles_NewsArticleArticleId",
                table: "BaseStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStocks_Users_UserCnp",
                table: "UserStocks");

            migrationBuilder.DropTable(
                name: "FavoriteStocks");

            migrationBuilder.DropTable(
                name: "NewsArticles");

            migrationBuilder.DropTable(
                name: "StockValues");

            migrationBuilder.DropIndex(
                name: "IX_BaseStocks_NewsArticleArticleId",
                table: "BaseStocks");

            migrationBuilder.DropColumn(
                name: "NewsArticleArticleId",
                table: "BaseStocks");
        }
    }
}
