using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppLaunch.Services.Migrations
{
    /// <inheritdoc />
    public partial class SeedColorSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "applaunch",
                table: "Sites",
                keyColumn: "SiteId",
                keyValue: new Guid("16c4e35d-1ce6-404d-9a61-c61b55f51a17"),
                columns: new[] { "ColorAppbarBackgroundDark", "ColorAppbarBackgroundLight", "ColorAppbarTextDark", "ColorAppbarTextLight", "ColorBackgroundDark", "ColorBackgroundLight", "ColorDrawerBackgroundDark", "ColorDrawerBackgroundLight", "ColorDrawerTextDark", "ColorDrawerTextLight", "ColorPrimaryDark", "ColorPrimaryLight", "ColorSecondaryDark", "ColorSecondaryLight", "ColorSurfaceDark", "ColorSurfaceLight", "ColorTextPrimaryDark", "ColorTextPrimaryLight", "ColorTextSecondaryDark", "ColorTextSecondaryLight" },
                values: new object[] { "#191a1a", "#191a1a", "#dadada", "#dadada", "#191a1a", "#F5F5F5", "#212222", "#212222", "#FFFFFF", "#FFFFFF", "#0081c4", "#0081c4", "#808080", "#808080", "#212222", "#FFFFFF", "#FFFFFF", "#202020", "#757575", "#757575" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "applaunch",
                table: "Sites",
                keyColumn: "SiteId",
                keyValue: new Guid("16c4e35d-1ce6-404d-9a61-c61b55f51a17"),
                columns: new[] { "ColorAppbarBackgroundDark", "ColorAppbarBackgroundLight", "ColorAppbarTextDark", "ColorAppbarTextLight", "ColorBackgroundDark", "ColorBackgroundLight", "ColorDrawerBackgroundDark", "ColorDrawerBackgroundLight", "ColorDrawerTextDark", "ColorDrawerTextLight", "ColorPrimaryDark", "ColorPrimaryLight", "ColorSecondaryDark", "ColorSecondaryLight", "ColorSurfaceDark", "ColorSurfaceLight", "ColorTextPrimaryDark", "ColorTextPrimaryLight", "ColorTextSecondaryDark", "ColorTextSecondaryLight" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null });
        }
    }
}
