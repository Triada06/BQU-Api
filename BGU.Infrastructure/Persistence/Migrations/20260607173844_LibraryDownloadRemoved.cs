using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LibraryDownloadRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadCount",
                table: "LibraryBooks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DownloadCount",
                table: "LibraryBooks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
