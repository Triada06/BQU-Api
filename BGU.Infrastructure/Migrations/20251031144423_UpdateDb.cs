using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentAcademicInfos_StudentAcademicInfoId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "AcademicInfoId",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "StudentAcademicInfoId",
                table: "Students",
                newName: "StudentAcademicInfoid");

            migrationBuilder.RenameIndex(
                name: "IX_Students_StudentAcademicInfoId",
                table: "Students",
                newName: "IX_Students_StudentAcademicInfoid");

            migrationBuilder.AlterColumn<string>(
                name: "StudentAcademicInfoid",
                table: "Students",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Gpa",
                table: "StudentAcademicInfos",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentAcademicInfos_StudentAcademicInfoid",
                table: "Students",
                column: "StudentAcademicInfoid",
                principalTable: "StudentAcademicInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentAcademicInfos_StudentAcademicInfoid",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Gpa",
                table: "StudentAcademicInfos");

            migrationBuilder.RenameColumn(
                name: "StudentAcademicInfoid",
                table: "Students",
                newName: "StudentAcademicInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_Students_StudentAcademicInfoid",
                table: "Students",
                newName: "IX_Students_StudentAcademicInfoId");

            migrationBuilder.AlterColumn<string>(
                name: "StudentAcademicInfoId",
                table: "Students",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "AcademicInfoId",
                table: "Students",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentAcademicInfos_StudentAcademicInfoId",
                table: "Students",
                column: "StudentAcademicInfoId",
                principalTable: "StudentAcademicInfos",
                principalColumn: "Id");
        }
    }
}
