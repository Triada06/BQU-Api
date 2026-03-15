using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyllabusCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Syllabus_TaughtSubjects_TaughtSubjectId",
                table: "Syllabus");

            migrationBuilder.AddForeignKey(
                name: "FK_Syllabus_TaughtSubjects_TaughtSubjectId",
                table: "Syllabus",
                column: "TaughtSubjectId",
                principalTable: "TaughtSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Syllabus_TaughtSubjects_TaughtSubjectId",
                table: "Syllabus");

            migrationBuilder.AddForeignKey(
                name: "FK_Syllabus_TaughtSubjects_TaughtSubjectId",
                table: "Syllabus",
                column: "TaughtSubjectId",
                principalTable: "TaughtSubjects",
                principalColumn: "Id");
        }
    }
}
