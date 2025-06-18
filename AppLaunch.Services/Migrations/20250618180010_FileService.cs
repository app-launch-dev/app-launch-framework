using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppLaunch.Services.Migrations
{
    /// <inheritdoc />
    public partial class FileService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                schema: "applaunch",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileBytes = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.FileId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files",
                schema: "applaunch");
        }
    }
}
