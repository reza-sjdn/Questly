using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Questly.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAllowAnonymousResponsesToSurveyAndUserIdToSurveyResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowAnonymousResponses",
                table: "Surveys",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "SurveyResponses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowAnonymousResponses",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SurveyResponses");
        }
    }
}
