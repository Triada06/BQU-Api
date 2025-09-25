using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ColloquiumTableNameUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colloquia_Students_StudentId",
                table: "Colloquia");

            migrationBuilder.DropForeignKey(
                name: "FK_Colloquia_TaughtSubjects_TaughtSubjectId",
                table: "Colloquia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Colloquia",
                table: "Colloquia");

            migrationBuilder.RenameTable(
                name: "Colloquia",
                newName: "Colloquiums");

            migrationBuilder.RenameIndex(
                name: "IX_Colloquia_TaughtSubjectId",
                table: "Colloquiums",
                newName: "IX_Colloquiums_TaughtSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Colloquia_StudentId",
                table: "Colloquiums",
                newName: "IX_Colloquiums_StudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Colloquiums",
                table: "Colloquiums",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Colloquiums_Students_StudentId",
                table: "Colloquiums",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Colloquiums_TaughtSubjects_TaughtSubjectId",
                table: "Colloquiums",
                column: "TaughtSubjectId",
                principalTable: "TaughtSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colloquiums_Students_StudentId",
                table: "Colloquiums");

            migrationBuilder.DropForeignKey(
                name: "FK_Colloquiums_TaughtSubjects_TaughtSubjectId",
                table: "Colloquiums");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Colloquiums",
                table: "Colloquiums");

            migrationBuilder.RenameTable(
                name: "Colloquiums",
                newName: "Colloquia");

            migrationBuilder.RenameIndex(
                name: "IX_Colloquiums_TaughtSubjectId",
                table: "Colloquia",
                newName: "IX_Colloquia_TaughtSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Colloquiums_StudentId",
                table: "Colloquia",
                newName: "IX_Colloquia_StudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Colloquia",
                table: "Colloquia",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Colloquia_Students_StudentId",
                table: "Colloquia",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Colloquia_TaughtSubjects_TaughtSubjectId",
                table: "Colloquia",
                column: "TaughtSubjectId",
                principalTable: "TaughtSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
