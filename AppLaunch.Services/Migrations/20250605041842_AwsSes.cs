using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppLaunch.Services.Migrations
{
    /// <inheritdoc />
    public partial class AwsSes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AwsSesAccessKey",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AwsSesSecretKey",
                schema: "applaunch",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                schema: "applaunch",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "applaunch",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                schema: "applaunch",
                table: "Sites",
                keyColumn: "SiteId",
                keyValue: new Guid("16c4e35d-1ce6-404d-9a61-c61b55f51a17"),
                columns: new[] { "AwsSesAccessKey", "AwsSesSecretKey" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwsSesAccessKey",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "AwsSesSecretKey",
                schema: "applaunch",
                table: "Sites");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                schema: "applaunch",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "applaunch",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
