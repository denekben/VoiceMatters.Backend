using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VoiceMatters.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "VoiceMatters",
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("20733a51-f270-4849-9b95-b2b61d0db7f8"));

            migrationBuilder.DeleteData(
                schema: "VoiceMatters",
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("6a04b26c-f4d1-4033-a9ef-0fbef464e91b"));

            migrationBuilder.DeleteData(
                schema: "VoiceMatters",
                table: "Statistics",
                keyColumn: "Id",
                keyValue: new Guid("cc15a4d9-2d26-4d30-bbca-38ce50abb8ed"));

            migrationBuilder.AlterColumn<string>(
                name: "Caption",
                schema: "VoiceMatters",
                table: "Images",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.InsertData(
                schema: "VoiceMatters",
                table: "Roles",
                columns: new[] { "Id", "CreatedDate", "RoleName", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("d63d2e2c-e6c8-4291-9c9b-5ab4fc73c1ef"), new DateTime(2025, 3, 12, 22, 29, 38, 623, DateTimeKind.Utc).AddTicks(5957), "Admin", null },
                    { new Guid("f2061a04-1a78-476e-bc1a-fc317fa5aeab"), new DateTime(2025, 3, 12, 22, 29, 38, 623, DateTimeKind.Utc).AddTicks(6879), "User", null }
                });

            migrationBuilder.InsertData(
                schema: "VoiceMatters",
                table: "Statistics",
                columns: new[] { "Id", "PetitionQuantity", "SignsQuantity", "UserQuantity" },
                values: new object[] { new Guid("9e298cdc-a737-42b2-98b3-daf1527539b1"), 0, 0, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "VoiceMatters",
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d63d2e2c-e6c8-4291-9c9b-5ab4fc73c1ef"));

            migrationBuilder.DeleteData(
                schema: "VoiceMatters",
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("f2061a04-1a78-476e-bc1a-fc317fa5aeab"));

            migrationBuilder.DeleteData(
                schema: "VoiceMatters",
                table: "Statistics",
                keyColumn: "Id",
                keyValue: new Guid("9e298cdc-a737-42b2-98b3-daf1527539b1"));

            migrationBuilder.AlterColumn<string>(
                name: "Caption",
                schema: "VoiceMatters",
                table: "Images",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.InsertData(
                schema: "VoiceMatters",
                table: "Roles",
                columns: new[] { "Id", "CreatedDate", "RoleName", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("20733a51-f270-4849-9b95-b2b61d0db7f8"), new DateTime(2025, 3, 10, 13, 48, 17, 284, DateTimeKind.Utc).AddTicks(2271), "User", null },
                    { new Guid("6a04b26c-f4d1-4033-a9ef-0fbef464e91b"), new DateTime(2025, 3, 10, 13, 48, 17, 284, DateTimeKind.Utc).AddTicks(1927), "Admin", null }
                });

            migrationBuilder.InsertData(
                schema: "VoiceMatters",
                table: "Statistics",
                columns: new[] { "Id", "PetitionQuantity", "SignsQuantity", "UserQuantity" },
                values: new object[] { new Guid("cc15a4d9-2d26-4d30-bbca-38ce50abb8ed"), 0, 0, 0 });
        }
    }
}
