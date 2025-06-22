using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppLaunch.Services.Migrations
{
    /// <inheritdoc />
    public partial class GitHubSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GitHubOwner",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitHubRepo",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitHubToken",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitHubWorkflowFile",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "applaunch",
                table: "Sites",
                keyColumn: "SiteId",
                keyValue: new Guid("16c4e35d-1ce6-404d-9a61-c61b55f51a17"),
                columns: new[] { "GitHubOwner", "GitHubRepo", "GitHubToken", "GitHubWorkflowFile" },
                values: new object[] { null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GitHubOwner",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "GitHubRepo",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "GitHubToken",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "GitHubWorkflowFile",
                schema: "applaunch",
                table: "Sites");
        }
    }
}
