using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppLaunch.Services.Migrations
{
    /// <inheritdoc />
    public partial class SettingsLogoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "applaunch",
                table: "Sites",
                keyColumn: "SiteId",
                keyValue: new Guid("16c4e35d-1ce6-404d-9a61-c61b55f51a17"),
                column: "LogoUrl",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                schema: "applaunch",
                table: "Sites");
        }
    }
}
