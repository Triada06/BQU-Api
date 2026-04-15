using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BGU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExamUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""Exams""
          ALTER COLUMN ""Grade"" TYPE integer
          USING NULLIF(""Grade"", '')::integer;"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""Exams""
          ALTER COLUMN ""Grade"" TYPE text
          USING ""Grade""::text;"
            );
        }
    }
}
