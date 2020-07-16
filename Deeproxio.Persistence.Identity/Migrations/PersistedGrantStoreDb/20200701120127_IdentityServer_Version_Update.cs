using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Deeproxio.Persistence.Identity.Migrations.PersistedGrantStoreDb
{
    public partial class IdentityServer_Version_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConsumedTime",
                schema: "id4_persist_grant",
                table: "PersistedGrants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "id4_persist_grant",
                table: "PersistedGrants",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                schema: "id4_persist_grant",
                table: "PersistedGrants",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "id4_persist_grant",
                table: "DeviceCodes",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                schema: "id4_persist_grant",
                table: "DeviceCodes",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId_SessionId_Type",
                schema: "id4_persist_grant",
                table: "PersistedGrants",
                columns: new[] { "SubjectId", "SessionId", "Type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PersistedGrants_SubjectId_SessionId_Type",
                schema: "id4_persist_grant",
                table: "PersistedGrants");

            migrationBuilder.DropColumn(
                name: "ConsumedTime",
                schema: "id4_persist_grant",
                table: "PersistedGrants");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "id4_persist_grant",
                table: "PersistedGrants");

            migrationBuilder.DropColumn(
                name: "SessionId",
                schema: "id4_persist_grant",
                table: "PersistedGrants");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "id4_persist_grant",
                table: "DeviceCodes");

            migrationBuilder.DropColumn(
                name: "SessionId",
                schema: "id4_persist_grant",
                table: "DeviceCodes");
        }
    }
}
