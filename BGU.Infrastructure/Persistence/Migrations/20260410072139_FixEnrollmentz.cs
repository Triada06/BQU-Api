using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixEnrollmentz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjectEnrollments_Groups_GroupId",
                table: "StudentSubjectEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_StudentSubjectEnrollments_GroupId",
                table: "StudentSubjectEnrollments");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "StudentSubjectEnrollments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "StudentSubjectEnrollments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjectEnrollments_GroupId",
                table: "StudentSubjectEnrollments",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjectEnrollments_Groups_GroupId",
                table: "StudentSubjectEnrollments",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
