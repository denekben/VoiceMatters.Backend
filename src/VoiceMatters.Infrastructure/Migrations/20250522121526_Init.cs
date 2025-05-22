using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VoiceMatters.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "VoiceMatters");

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "VoiceMatters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleName = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                schema: "VoiceMatters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserQuantity = table.Column<int>(type: "integer", nullable: false),
                    PetitionQuantity = table.Column<int>(type: "integer", nullable: false),
                    SignsQuantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "VoiceMatters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "VoiceMatters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Sex = table.Column<string>(type: "text", nullable: true),
                    ImageUuid = table.Column<string>(type: "text", nullable: true),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpires = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "VoiceMatters",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Petitions",
                schema: "VoiceMatters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    TextPayload = table.Column<string>(type: "text", nullable: false),
                    SignQuantity = table.Column<long>(type: "bigint", nullable: false),
                    SignQuantityPerDay = table.Column<long>(type: "bigint", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Petitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Petitions_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalSchema: "VoiceMatters",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AppUserSignedPetitions",
                schema: "VoiceMatters",
                columns: table => new
                {
                    SignerId = table.Column<Guid>(type: "uuid", nullable: false),
                    PetitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SignedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserSignedPetitions", x => new { x.PetitionId, x.SignerId });
                    table.ForeignKey(
                        name: "FK_AppUserSignedPetitions_Petitions_PetitionId",
                        column: x => x.PetitionId,
                        principalSchema: "VoiceMatters",
                        principalTable: "Petitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserSignedPetitions_Users_SignerId",
                        column: x => x.SignerId,
                        principalSchema: "VoiceMatters",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                schema: "VoiceMatters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Uuid = table.Column<string>(type: "text", nullable: false),
                    Caption = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PetitionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_Petitions_PetitionId",
                        column: x => x.PetitionId,
                        principalSchema: "VoiceMatters",
                        principalTable: "Petitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "News",
                schema: "VoiceMatters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PetitionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                    table.ForeignKey(
                        name: "FK_News_Petitions_PetitionId",
                        column: x => x.PetitionId,
                        principalSchema: "VoiceMatters",
                        principalTable: "Petitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostTags",
                schema: "VoiceMatters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PetitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostTags_Petitions_PetitionId",
                        column: x => x.PetitionId,
                        principalSchema: "VoiceMatters",
                        principalTable: "Petitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "VoiceMatters",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "VoiceMatters",
                table: "Roles",
                columns: new[] { "Id", "CreatedDate", "RoleName", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("cf27880a-14ef-4013-9955-12356f97f927"), new DateTime(2025, 5, 22, 12, 15, 25, 534, DateTimeKind.Utc).AddTicks(4183), "User", null },
                    { new Guid("d539b807-c559-4ad9-be9a-897bf38e543a"), new DateTime(2025, 5, 22, 12, 15, 25, 534, DateTimeKind.Utc).AddTicks(2940), "Admin", null }
                });

            migrationBuilder.InsertData(
                schema: "VoiceMatters",
                table: "Statistics",
                columns: new[] { "Id", "PetitionQuantity", "SignsQuantity", "UserQuantity" },
                values: new object[] { new Guid("19e03bf6-4bd8-4ecd-8aa8-819167715efd"), 0, 0, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserSignedPetitions_SignerId",
                schema: "VoiceMatters",
                table: "AppUserSignedPetitions",
                column: "SignerId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_PetitionId",
                schema: "VoiceMatters",
                table: "Images",
                column: "PetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_News_PetitionId",
                schema: "VoiceMatters",
                table: "News",
                column: "PetitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Petitions_CreatorId",
                schema: "VoiceMatters",
                table: "Petitions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_PetitionId",
                schema: "VoiceMatters",
                table: "PostTags",
                column: "PetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_TagId",
                schema: "VoiceMatters",
                table: "PostTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                schema: "VoiceMatters",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUserSignedPetitions",
                schema: "VoiceMatters");

            migrationBuilder.DropTable(
                name: "Images",
                schema: "VoiceMatters");

            migrationBuilder.DropTable(
                name: "News",
                schema: "VoiceMatters");

            migrationBuilder.DropTable(
                name: "PostTags",
                schema: "VoiceMatters");

            migrationBuilder.DropTable(
                name: "Statistics",
                schema: "VoiceMatters");

            migrationBuilder.DropTable(
                name: "Petitions",
                schema: "VoiceMatters");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "VoiceMatters");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "VoiceMatters");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "VoiceMatters");
        }
    }
}
