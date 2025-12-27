using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ClassTimeModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_ClassSessions_ClassSessionId",
                table: "Attendances");

            migrationBuilder.DropTable(
                name: "ClassSessions");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_ClassSessionId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "ClassSessionId",
                table: "Attendances");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ClassDate",
                table: "ClassTimes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "IsUpperWeek",
                table: "ClassTimes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "Classes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassDate",
                table: "ClassTimes");

            migrationBuilder.DropColumn(
                name: "IsUpperWeek",
                table: "ClassTimes");

            migrationBuilder.DropColumn(
                name: "Room",
                table: "Classes");

            migrationBuilder.AddColumn<string>(
                name: "ClassSessionId",
                table: "Attendances",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClassSessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    TaughtSubjectId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ClassSessionId",
                table: "Attendances",
                column: "ClassSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_TaughtSubjectId",
                table: "ClassSessions",
                column: "TaughtSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_ClassSessions_ClassSessionId",
                table: "Attendances",
                column: "ClassSessionId",
                principalTable: "ClassSessions",
                principalColumn: "Id");
        }
    }
}
