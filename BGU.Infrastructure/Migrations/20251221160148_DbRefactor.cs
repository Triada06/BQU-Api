using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DbRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "TeacherAcademicInfos");

            migrationBuilder.DropColumn(
                name: "TypeOfContract",
                table: "TeacherAcademicInfos");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "LectureHalls");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Deans",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Pin",
                table: "AspNetUsers",
                column: "Pin",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Pin",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Deans");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "TeacherAcademicInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeOfContract",
                table: "TeacherAcademicInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "LectureHalls",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
