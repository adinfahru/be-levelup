using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUp.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpFieldsToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "otp",
                table: "accounts",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "otp_attempts",
                table: "accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "otp_expires_at",
                table: "accounts",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "otp_attempts",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "otp_expires_at",
                table: "accounts");

            migrationBuilder.AlterColumn<int>(
                name: "otp",
                table: "accounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);
        }
    }
}
