using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExamIsAllowed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAllowed",
                table: "Exams",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAllowed",
                table: "Exams");
        }
    }
}
