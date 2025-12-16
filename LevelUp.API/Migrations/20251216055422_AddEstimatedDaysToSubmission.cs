using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUp.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimatedDaysToSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstimatedDays",
                table: "submissions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedDays",
                table: "submissions");
        }
    }
}
