using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attendance",
                table: "AcademicPerformances");

            migrationBuilder.AddColumn<int>(
                name: "Hours",
                table: "TaughtSubjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPassed",
                table: "IndependentWorks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AttendanceId",
                table: "AcademicPerformances",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClassSessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    TaughtSubjectId = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassSessions_TaughtSubjects_TaughtSubjectId",
                        column: x => x.TaughtSubjectId,
                        principalTable: "TaughtSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seminars",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    StudentId = table.Column<string>(type: "text", nullable: false),
                    TaughtSubjectId = table.Column<string>(type: "text", nullable: false),
                    GotAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Grade = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seminars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seminars_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Seminars_TaughtSubjects_TaughtSubjectId",
                        column: x => x.TaughtSubjectId,
                        principalTable: "TaughtSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    StudentId = table.Column<string>(type: "text", nullable: false),
                    ClassId = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAbsent = table.Column<bool>(type: "boolean", nullable: false),
                    ClassSessionId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_ClassSessions_ClassSessionId",
                        column: x => x.ClassSessionId,
                        principalTable: "ClassSessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Attendances_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendances_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicPerformances_AttendanceId",
                table: "AcademicPerformances",
                column: "AttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ClassId",
                table: "Attendances",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ClassSessionId",
                table: "Attendances",
                column: "ClassSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentId",
                table: "Attendances",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_TaughtSubjectId",
                table: "ClassSessions",
                column: "TaughtSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Seminars_StudentId",
                table: "Seminars",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Seminars_TaughtSubjectId",
                table: "Seminars",
                column: "TaughtSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicPerformances_Attendances_AttendanceId",
                table: "AcademicPerformances",
                column: "AttendanceId",
                principalTable: "Attendances",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicPerformances_Attendances_AttendanceId",
                table: "AcademicPerformances");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "Seminars");

            migrationBuilder.DropTable(
                name: "ClassSessions");

            migrationBuilder.DropIndex(
                name: "IX_AcademicPerformances_AttendanceId",
                table: "AcademicPerformances");

            migrationBuilder.DropColumn(
                name: "Hours",
                table: "TaughtSubjects");

            migrationBuilder.DropColumn(
                name: "IsPassed",
                table: "IndependentWorks");

            migrationBuilder.DropColumn(
                name: "AttendanceId",
                table: "AcademicPerformances");

            migrationBuilder.AddColumn<int>(
                name: "Attendance",
                table: "AcademicPerformances",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
