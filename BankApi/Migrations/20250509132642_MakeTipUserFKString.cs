using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankApi.Migrations
{
    /// <inheritdoc />
    public partial class MakeTipUserFKString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GivenTips_Users_UserCNP",
                table: "GivenTips");

            migrationBuilder.AlterColumn<string>(
                name: "UserCNP",
                table: "GivenTips",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 13);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Users_CNP",
                table: "Users",
                column: "CNP");

            migrationBuilder.AddForeignKey(
                name: "FK_GivenTips_Users_UserCNP",
                table: "GivenTips",
                column: "UserCNP",
                principalTable: "Users",
                principalColumn: "CNP",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GivenTips_Users_UserCNP",
                table: "GivenTips");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Users_CNP",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "UserCNP",
                table: "GivenTips",
                type: "int",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.AddForeignKey(
                name: "FK_GivenTips_Users_UserCNP",
                table: "GivenTips",
                column: "UserCNP",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
