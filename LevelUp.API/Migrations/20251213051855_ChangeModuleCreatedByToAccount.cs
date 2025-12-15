using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUp.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeModuleCreatedByToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_modules_employees_created_by",
                table: "modules");

            migrationBuilder.AddForeignKey(
                name: "FK_modules_accounts_created_by",
                table: "modules",
                column: "created_by",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_modules_accounts_created_by",
                table: "modules");

            migrationBuilder.AddForeignKey(
                name: "FK_modules_employees_created_by",
                table: "modules",
                column: "created_by",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
