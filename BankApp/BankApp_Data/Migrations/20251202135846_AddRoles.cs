using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankApp_Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Accountants_AccountantId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "AccountantId",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Accountants_AccountantId",
                table: "Users",
                column: "AccountantId",
                principalTable: "Accountants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Accountants_AccountantId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "AccountantId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Accountants_AccountantId",
                table: "Users",
                column: "AccountantId",
                principalTable: "Accountants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
