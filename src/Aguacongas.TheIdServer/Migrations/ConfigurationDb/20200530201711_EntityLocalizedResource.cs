using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.Migrations.ConfigurationDb
{
    public partial class EntityLocalizedResource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalClaimTransformation_Providers_Scheme",
                table: "ExternalClaimTransformation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExternalClaimTransformation",
                table: "ExternalClaimTransformation");

            migrationBuilder.RenameTable(
                name: "ExternalClaimTransformation",
                newName: "ExternalClaimTransformations");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalClaimTransformation_Scheme_FromClaimType",
                table: "ExternalClaimTransformations",
                newName: "IX_ExternalClaimTransformations_Scheme_FromClaimType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalClaimTransformations",
                table: "ExternalClaimTransformations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ApiLocalizedResources",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    CultureId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ApiId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiLocalizedResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiLocalizedResources_Apis_ApiId",
                        column: x => x.ApiId,
                        principalTable: "Apis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApiLocalizedResources_Cultures_CultureId",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientLocalizedResources",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    CultureId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ClientId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLocalizedResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientLocalizedResources_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientLocalizedResources_Cultures_CultureId",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityLocalizedResources",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    CultureId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    IdentityId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityLocalizedResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityLocalizedResources_Cultures_CultureId",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityLocalizedResources_Identities_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "Identities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en-US",
                column: "CreatedAt",
                value: new DateTime(2020, 5, 30, 20, 17, 11, 305, DateTimeKind.Utc).AddTicks(2599));

            migrationBuilder.CreateIndex(
                name: "IX_ApiLocalizedResources_ApiId",
                table: "ApiLocalizedResources",
                column: "ApiId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiLocalizedResources_CultureId",
                table: "ApiLocalizedResources",
                column: "CultureId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientLocalizedResources_ClientId",
                table: "ClientLocalizedResources",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientLocalizedResources_CultureId",
                table: "ClientLocalizedResources",
                column: "CultureId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityLocalizedResources_CultureId",
                table: "IdentityLocalizedResources",
                column: "CultureId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityLocalizedResources_IdentityId",
                table: "IdentityLocalizedResources",
                column: "IdentityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalClaimTransformations_Providers_Scheme",
                table: "ExternalClaimTransformations",
                column: "Scheme",
                principalTable: "Providers",
                principalColumn: "Scheme",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalClaimTransformations_Providers_Scheme",
                table: "ExternalClaimTransformations");

            migrationBuilder.DropTable(
                name: "ApiLocalizedResources");

            migrationBuilder.DropTable(
                name: "ClientLocalizedResources");

            migrationBuilder.DropTable(
                name: "IdentityLocalizedResources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExternalClaimTransformations",
                table: "ExternalClaimTransformations");

            migrationBuilder.RenameTable(
                name: "ExternalClaimTransformations",
                newName: "ExternalClaimTransformation");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalClaimTransformations_Scheme_FromClaimType",
                table: "ExternalClaimTransformation",
                newName: "IX_ExternalClaimTransformation_Scheme_FromClaimType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalClaimTransformation",
                table: "ExternalClaimTransformation",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en-US",
                column: "CreatedAt",
                value: new DateTime(2020, 5, 27, 20, 21, 25, 310, DateTimeKind.Utc).AddTicks(2838));

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalClaimTransformation_Providers_Scheme",
                table: "ExternalClaimTransformation",
                column: "Scheme",
                principalTable: "Providers",
                principalColumn: "Scheme",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
