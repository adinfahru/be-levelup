using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUp.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimatedDaysToSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EstimatedDays",
                table: "submissions",
                newName: "estimated_days");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "estimated_days",
                table: "submissions",
                newName: "EstimatedDays");
        }
    }
}
