using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Enrolments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentSubjectEnrollments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    StudentId = table.Column<string>(type: "text", nullable: false),
                    GroupId = table.Column<string>(type: "text", nullable: false),
                    TaughtSubjectId = table.Column<string>(type: "text", nullable: false),
                    Attempt = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSubjectEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentSubjectEnrollments_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSubjectEnrollments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSubjectEnrollments_TaughtSubjects_TaughtSubjectId",
                        column: x => x.TaughtSubjectId,
                        principalTable: "TaughtSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjectEnrollments_GroupId",
                table: "StudentSubjectEnrollments",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjectEnrollments_StudentId",
                table: "StudentSubjectEnrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjectEnrollments_TaughtSubjectId",
                table: "StudentSubjectEnrollments",
                column: "TaughtSubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentSubjectEnrollments");
        }
    }
}
