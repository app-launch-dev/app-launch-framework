using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppLaunch.Services.Migrations
{
    /// <inheritdoc />
    public partial class FirstLastMidlleUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "applaunch",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "applaunch",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                schema: "applaunch",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "applaunch",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "applaunch",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                schema: "applaunch",
                table: "AspNetUsers");
        }
    }
}
