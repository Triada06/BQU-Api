using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactoringStudentAndGroupRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DecreeNumber",
                table: "StudentAcademicInfos");

            migrationBuilder.DropColumn(
                name: "FormOfEducation",
                table: "StudentAcademicInfos");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DecreeNumber",
                table: "StudentAcademicInfos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FormOfEducation",
                table: "StudentAcademicInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<char>(
                name: "Gender",
                table: "AspNetUsers",
                type: "character(1)",
                nullable: false,
                defaultValue: '\0');
        }
    }
}
