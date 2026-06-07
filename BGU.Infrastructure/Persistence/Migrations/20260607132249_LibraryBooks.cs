using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LibraryBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LibraryBooks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Authors = table.Column<string[]>(type: "text[]", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Isbn = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Publisher = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    PublishedYear = table.Column<int>(type: "integer", nullable: true),
                    Edition = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Tags = table.Column<string[]>(type: "text[]", nullable: false),
                    Format = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CoverImageFileName = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: true),
                    StoredFileName = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: true),
                    FileName = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    FileContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DownloadCount = table.Column<int>(type: "integer", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibraryBooks_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LibraryBooks_CreatedById",
                table: "LibraryBooks",
                column: "CreatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LibraryBooks");
        }
    }
}
