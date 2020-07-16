using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Deeproxio.Persistence.Identity.Migrations.ConfigurationStoreDb
{
    public partial class IdentityServer_Version_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiClaims_ApiResources_ApiResourceId",
                schema: "id4_config",
                table: "ApiClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiProperties_ApiResources_ApiResourceId",
                schema: "id4_config",
                table: "ApiProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiScopeClaims_ApiScopes_ApiScopeId",
                schema: "id4_config",
                table: "ApiScopeClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiScopes_ApiResources_ApiResourceId",
                schema: "id4_config",
                table: "ApiScopes");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityProperties_IdentityResources_IdentityResourceId",
                schema: "id4_config",
                table: "IdentityProperties");

            migrationBuilder.DropTable(
                name: "ApiSecrets",
                schema: "id4_config");

            migrationBuilder.DropTable(
                name: "IdentityClaims",
                schema: "id4_config");

            migrationBuilder.DropIndex(
                name: "IX_ApiScopes_ApiResourceId",
                schema: "id4_config",
                table: "ApiScopes");

            migrationBuilder.DropIndex(
                name: "IX_ApiScopeClaims_ApiScopeId",
                schema: "id4_config",
                table: "ApiScopeClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityProperties",
                schema: "id4_config",
                table: "IdentityProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiProperties",
                schema: "id4_config",
                table: "ApiProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiClaims",
                schema: "id4_config",
                table: "ApiClaims");

            migrationBuilder.DropColumn(
                name: "ApiResourceId",
                schema: "id4_config",
                table: "ApiScopes");

            migrationBuilder.DropColumn(
                name: "ApiScopeId",
                schema: "id4_config",
                table: "ApiScopeClaims");

            migrationBuilder.RenameTable(
                name: "IdentityProperties",
                schema: "id4_config",
                newName: "IdentityResourceProperties",
                newSchema: "id4_config");

            migrationBuilder.RenameTable(
                name: "ApiProperties",
                schema: "id4_config",
                newName: "ApiResourceProperties",
                newSchema: "id4_config");

            migrationBuilder.RenameTable(
                name: "ApiClaims",
                schema: "id4_config",
                newName: "ApiResourceClaims",
                newSchema: "id4_config");

            migrationBuilder.RenameIndex(
                name: "IX_IdentityProperties_IdentityResourceId",
                schema: "id4_config",
                table: "IdentityResourceProperties",
                newName: "IX_IdentityResourceProperties_IdentityResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_ApiProperties_ApiResourceId",
                schema: "id4_config",
                table: "ApiResourceProperties",
                newName: "IX_ApiResourceProperties_ApiResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_ApiClaims_ApiResourceId",
                schema: "id4_config",
                table: "ApiResourceClaims",
                newName: "IX_ApiResourceClaims_ApiResourceId");

            migrationBuilder.AddColumn<string>(
                name: "AllowedIdentityTokenSigningAlgorithms",
                schema: "id4_config",
                table: "Clients",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireRequestObject",
                schema: "id4_config",
                table: "Clients",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                schema: "id4_config",
                table: "ApiScopes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ScopeId",
                schema: "id4_config",
                table: "ApiScopeClaims",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AllowedAccessTokenSigningAlgorithms",
                schema: "id4_config",
                table: "ApiResources",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowInDiscoveryDocument",
                schema: "id4_config",
                table: "ApiResources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityResourceProperties",
                schema: "id4_config",
                table: "IdentityResourceProperties",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiResourceProperties",
                schema: "id4_config",
                table: "ApiResourceProperties",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiResourceClaims",
                schema: "id4_config",
                table: "ApiResourceClaims",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ApiResourceScopes",
                schema: "id4_config",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Scope = table.Column<string>(maxLength: 200, nullable: false),
                    ApiResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiResourceScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiResourceScopes_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalSchema: "id4_config",
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiResourceSecrets",
                schema: "id4_config",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Value = table.Column<string>(maxLength: 4000, nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(maxLength: 250, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    ApiResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiResourceSecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiResourceSecrets_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalSchema: "id4_config",
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiScopeProperties",
                schema: "id4_config",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    ScopeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiScopeProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiScopeProperties_ApiScopes_ScopeId",
                        column: x => x.ScopeId,
                        principalSchema: "id4_config",
                        principalTable: "ApiScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityResourceClaims",
                schema: "id4_config",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Type = table.Column<string>(maxLength: 200, nullable: false),
                    IdentityResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityResourceClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityResourceClaims_IdentityResources_IdentityResourceId",
                        column: x => x.IdentityResourceId,
                        principalSchema: "id4_config",
                        principalTable: "IdentityResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeClaims_ScopeId",
                schema: "id4_config",
                table: "ApiScopeClaims",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceScopes_ApiResourceId",
                schema: "id4_config",
                table: "ApiResourceScopes",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceSecrets_ApiResourceId",
                schema: "id4_config",
                table: "ApiResourceSecrets",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeProperties_ScopeId",
                schema: "id4_config",
                table: "ApiScopeProperties",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityResourceClaims_IdentityResourceId",
                schema: "id4_config",
                table: "IdentityResourceClaims",
                column: "IdentityResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiResourceClaims_ApiResources_ApiResourceId",
                schema: "id4_config",
                table: "ApiResourceClaims",
                column: "ApiResourceId",
                principalSchema: "id4_config",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiResourceProperties_ApiResources_ApiResourceId",
                schema: "id4_config",
                table: "ApiResourceProperties",
                column: "ApiResourceId",
                principalSchema: "id4_config",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiScopeClaims_ApiScopes_ScopeId",
                schema: "id4_config",
                table: "ApiScopeClaims",
                column: "ScopeId",
                principalSchema: "id4_config",
                principalTable: "ApiScopes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityResourceProperties_IdentityResources_IdentityResour~",
                schema: "id4_config",
                table: "IdentityResourceProperties",
                column: "IdentityResourceId",
                principalSchema: "id4_config",
                principalTable: "IdentityResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiResourceClaims_ApiResources_ApiResourceId",
                schema: "id4_config",
                table: "ApiResourceClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiResourceProperties_ApiResources_ApiResourceId",
                schema: "id4_config",
                table: "ApiResourceProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiScopeClaims_ApiScopes_ScopeId",
                schema: "id4_config",
                table: "ApiScopeClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityResourceProperties_IdentityResources_IdentityResour~",
                schema: "id4_config",
                table: "IdentityResourceProperties");

            migrationBuilder.DropTable(
                name: "ApiResourceScopes",
                schema: "id4_config");

            migrationBuilder.DropTable(
                name: "ApiResourceSecrets",
                schema: "id4_config");

            migrationBuilder.DropTable(
                name: "ApiScopeProperties",
                schema: "id4_config");

            migrationBuilder.DropTable(
                name: "IdentityResourceClaims",
                schema: "id4_config");

            migrationBuilder.DropIndex(
                name: "IX_ApiScopeClaims_ScopeId",
                schema: "id4_config",
                table: "ApiScopeClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityResourceProperties",
                schema: "id4_config",
                table: "IdentityResourceProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiResourceProperties",
                schema: "id4_config",
                table: "ApiResourceProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiResourceClaims",
                schema: "id4_config",
                table: "ApiResourceClaims");

            migrationBuilder.DropColumn(
                name: "AllowedIdentityTokenSigningAlgorithms",
                schema: "id4_config",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "RequireRequestObject",
                schema: "id4_config",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Enabled",
                schema: "id4_config",
                table: "ApiScopes");

            migrationBuilder.DropColumn(
                name: "ScopeId",
                schema: "id4_config",
                table: "ApiScopeClaims");

            migrationBuilder.DropColumn(
                name: "AllowedAccessTokenSigningAlgorithms",
                schema: "id4_config",
                table: "ApiResources");

            migrationBuilder.DropColumn(
                name: "ShowInDiscoveryDocument",
                schema: "id4_config",
                table: "ApiResources");

            migrationBuilder.RenameTable(
                name: "IdentityResourceProperties",
                schema: "id4_config",
                newName: "IdentityProperties",
                newSchema: "id4_config");

            migrationBuilder.RenameTable(
                name: "ApiResourceProperties",
                schema: "id4_config",
                newName: "ApiProperties",
                newSchema: "id4_config");

            migrationBuilder.RenameTable(
                name: "ApiResourceClaims",
                schema: "id4_config",
                newName: "ApiClaims",
                newSchema: "id4_config");

            migrationBuilder.RenameIndex(
                name: "IX_IdentityResourceProperties_IdentityResourceId",
                schema: "id4_config",
                table: "IdentityProperties",
                newName: "IX_IdentityProperties_IdentityResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_ApiResourceProperties_ApiResourceId",
                schema: "id4_config",
                table: "ApiProperties",
                newName: "IX_ApiProperties_ApiResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_ApiResourceClaims_ApiResourceId",
                schema: "id4_config",
                table: "ApiClaims",
                newName: "IX_ApiClaims_ApiResourceId");

            migrationBuilder.AddColumn<int>(
                name: "ApiResourceId",
                schema: "id4_config",
                table: "ApiScopes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApiScopeId",
                schema: "id4_config",
                table: "ApiScopeClaims",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityProperties",
                schema: "id4_config",
                table: "IdentityProperties",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiProperties",
                schema: "id4_config",
                table: "ApiProperties",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiClaims",
                schema: "id4_config",
                table: "ApiClaims",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ApiSecrets",
                schema: "id4_config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ApiResourceId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Expiration = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Type = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiSecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiSecrets_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalSchema: "id4_config",
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityClaims",
                schema: "id4_config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    IdentityResourceId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityClaims_IdentityResources_IdentityResourceId",
                        column: x => x.IdentityResourceId,
                        principalSchema: "id4_config",
                        principalTable: "IdentityResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopes_ApiResourceId",
                schema: "id4_config",
                table: "ApiScopes",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeClaims_ApiScopeId",
                schema: "id4_config",
                table: "ApiScopeClaims",
                column: "ApiScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiSecrets_ApiResourceId",
                schema: "id4_config",
                table: "ApiSecrets",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityClaims_IdentityResourceId",
                schema: "id4_config",
                table: "IdentityClaims",
                column: "IdentityResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiClaims_ApiResources_ApiResourceId",
                schema: "id4_config",
                table: "ApiClaims",
                column: "ApiResourceId",
                principalSchema: "id4_config",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiProperties_ApiResources_ApiResourceId",
                schema: "id4_config",
                table: "ApiProperties",
                column: "ApiResourceId",
                principalSchema: "id4_config",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiScopeClaims_ApiScopes_ApiScopeId",
                schema: "id4_config",
                table: "ApiScopeClaims",
                column: "ApiScopeId",
                principalSchema: "id4_config",
                principalTable: "ApiScopes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiScopes_ApiResources_ApiResourceId",
                schema: "id4_config",
                table: "ApiScopes",
                column: "ApiResourceId",
                principalSchema: "id4_config",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityProperties_IdentityResources_IdentityResourceId",
                schema: "id4_config",
                table: "IdentityProperties",
                column: "IdentityResourceId",
                principalSchema: "id4_config",
                principalTable: "IdentityResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
