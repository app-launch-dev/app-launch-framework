using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppLaunch.Services.Migrations
{
    /// <inheritdoc />
    public partial class SeededRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "applaunch",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0b5b6765-40c8-410a-9271-92651118a908", null, "Manager", "MANAGER" },
                    { "21ab5e5f-43e1-40eb-9bef-b26ac0dc6751", null, "Admin", "ADMIN" },
                    { "d68c4c67-c79b-4f43-8795-feddccaf0832", null, "Public", "PUBLIC" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "applaunch",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b5b6765-40c8-410a-9271-92651118a908");

            migrationBuilder.DeleteData(
                schema: "applaunch",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "21ab5e5f-43e1-40eb-9bef-b26ac0dc6751");

            migrationBuilder.DeleteData(
                schema: "applaunch",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d68c4c67-c79b-4f43-8795-feddccaf0832");
        }
    }
}
