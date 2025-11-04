using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAcademicInfoRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentAcademicInfos_StudentAcademicInfoid",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_TeacherAcademicInfos_TeacherAcademicInfoId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_TeacherAcademicInfoId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Students_StudentAcademicInfoid",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "TeacherAcademicInfoId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "StudentAcademicInfoid",
                table: "Students");

            migrationBuilder.AddColumn<string>(
                name: "TeacherId",
                table: "TeacherAcademicInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentId",
                table: "StudentAcademicInfos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAcademicInfos_TeacherId",
                table: "TeacherAcademicInfos",
                column: "TeacherId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAcademicInfos_StudentId",
                table: "StudentAcademicInfos",
                column: "StudentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAcademicInfos_Students_StudentId",
                table: "StudentAcademicInfos",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherAcademicInfos_Teachers_TeacherId",
                table: "TeacherAcademicInfos",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAcademicInfos_Students_StudentId",
                table: "StudentAcademicInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherAcademicInfos_Teachers_TeacherId",
                table: "TeacherAcademicInfos");

            migrationBuilder.DropIndex(
                name: "IX_TeacherAcademicInfos_TeacherId",
                table: "TeacherAcademicInfos");

            migrationBuilder.DropIndex(
                name: "IX_StudentAcademicInfos_StudentId",
                table: "StudentAcademicInfos");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "TeacherAcademicInfos");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "StudentAcademicInfos");

            migrationBuilder.AddColumn<string>(
                name: "TeacherAcademicInfoId",
                table: "Teachers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentAcademicInfoid",
                table: "Students",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_TeacherAcademicInfoId",
                table: "Teachers",
                column: "TeacherAcademicInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentAcademicInfoid",
                table: "Students",
                column: "StudentAcademicInfoid");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentAcademicInfos_StudentAcademicInfoid",
                table: "Students",
                column: "StudentAcademicInfoid",
                principalTable: "StudentAcademicInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_TeacherAcademicInfos_TeacherAcademicInfoId",
                table: "Teachers",
                column: "TeacherAcademicInfoId",
                principalTable: "TeacherAcademicInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
