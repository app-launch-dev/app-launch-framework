using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppLaunch.Services.Migrations
{
    /// <inheritdoc />
    public partial class ColorSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorAppbarBackgroundDark",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorAppbarBackgroundLight",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorAppbarTextDark",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorAppbarTextLight",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorBackgroundDark",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorBackgroundLight",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorDrawerBackgroundDark",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorDrawerBackgroundLight",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorDrawerTextDark",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorDrawerTextLight",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorPrimaryDark",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorPrimaryLight",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorSecondaryDark",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorSecondaryLight",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorSurfaceDark",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorSurfaceLight",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorTextPrimaryDark",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorTextPrimaryLight",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorTextSecondaryDark",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorTextSecondaryLight",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "applaunch",
                table: "Sites",
                keyColumn: "SiteId",
                keyValue: new Guid("16c4e35d-1ce6-404d-9a61-c61b55f51a17"),
                columns: new[] { "ColorAppbarBackgroundDark", "ColorAppbarBackgroundLight", "ColorAppbarTextDark", "ColorAppbarTextLight", "ColorBackgroundDark", "ColorBackgroundLight", "ColorDrawerBackgroundDark", "ColorDrawerBackgroundLight", "ColorDrawerTextDark", "ColorDrawerTextLight", "ColorPrimaryDark", "ColorPrimaryLight", "ColorSecondaryDark", "ColorSecondaryLight", "ColorSurfaceDark", "ColorSurfaceLight", "ColorTextPrimaryDark", "ColorTextPrimaryLight", "ColorTextSecondaryDark", "ColorTextSecondaryLight" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorAppbarBackgroundDark",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorAppbarBackgroundLight",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorAppbarTextDark",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorAppbarTextLight",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorBackgroundDark",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorBackgroundLight",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorDrawerBackgroundDark",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorDrawerBackgroundLight",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorDrawerTextDark",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorDrawerTextLight",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorPrimaryDark",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorPrimaryLight",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorSecondaryDark",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorSecondaryLight",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorSurfaceDark",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorSurfaceLight",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorTextPrimaryDark",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorTextPrimaryLight",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorTextSecondaryDark",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ColorTextSecondaryLight",
                schema: "applaunch",
                table: "Sites");
        }
    }
}
