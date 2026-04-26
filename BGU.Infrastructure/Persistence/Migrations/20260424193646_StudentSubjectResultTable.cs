using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StudentSubjectResultTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentSubjectResults",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    StudentId = table.Column<string>(type: "text", nullable: false),
                    TaughtSubjectId = table.Column<string>(type: "text", nullable: false),
                    FinalGrade = table.Column<double>(type: "double precision", nullable: false),
                    IsFinalized = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSubjectResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentSubjectResults_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSubjectResults_TaughtSubjects_TaughtSubjectId",
                        column: x => x.TaughtSubjectId,
                        principalTable: "TaughtSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjectResults_StudentId",
                table: "StudentSubjectResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjectResults_TaughtSubjectId",
                table: "StudentSubjectResults",
                column: "TaughtSubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentSubjectResults");
        }
    }
}
