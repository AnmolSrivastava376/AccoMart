using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class newazure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "93cb5859-0603-4485-86a5-79322d2a0b75");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bb004458-4c73-4737-ab6e-5e38cf8341fe");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a13a4584-fb15-4d7a-bc45-f7863dadbd21", "1", "Admin", "Admin" },
                    { "f42e5529-0538-4332-9be9-d5285e70d4a5", "1", "User", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a13a4584-fb15-4d7a-bc45-f7863dadbd21");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f42e5529-0538-4332-9be9-d5285e70d4a5");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "93cb5859-0603-4485-86a5-79322d2a0b75", "1", "Admin", "Admin" },
                    { "bb004458-4c73-4737-ab6e-5e38cf8341fe", "1", "User", "User" }
                });
        }
    }
}
