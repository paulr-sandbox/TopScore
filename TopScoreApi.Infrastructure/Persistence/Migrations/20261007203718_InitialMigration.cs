using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopScoreApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestScores",
                columns: table => new
                {
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestScores", x => new { x.FirstName, x.LastName });
                    table.CheckConstraint("CK_TestScore_FirstName_NotEmpty", "length(FirstName) > 0");
                    table.CheckConstraint("CK_TestScore_LastName_NotEmpty", "length(LastName) > 0");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestScores");
        }
    }
}
