using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppLaunch.Services.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "applaunch",
                table: "Sites",
                columns: new[] { "SiteId", "AllowSignUp", "ContentKey", "DefaultEmailFrom", "GlobalContent", "Layout", "LoginLogoUrl", "LoginOverrideCSS", "OverrideCSS", "SiteName", "ThemeId" },
                values: new object[] { new Guid("16c4e35d-1ce6-404d-9a61-c61b55f51a17"), false, "f15998d5-c4b7-4719-b0b4-c86761ce9ae8", "no-reply@mydomain.com", null, "AppLaunchDefault.razor", null, null, "", "My AppLaunch Site", new Guid("d145c1f7-2193-473a-aa6f-3ceed8343a44") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "applaunch",
                table: "Sites",
                keyColumn: "SiteId",
                keyValue: new Guid("16c4e35d-1ce6-404d-9a61-c61b55f51a17"));
        }
    }
}
