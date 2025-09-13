// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.Sqlite.Migrations.ConfigurationDb
{
    public partial class WsFederation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RelyingParties",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    TokenType = table.Column<string>(type: "TEXT", nullable: false),
                    DigestAlgorithm = table.Column<string>(type: "TEXT", nullable: false),
                    SignatureAlgorithm = table.Column<string>(type: "TEXT", nullable: false),
                    SamlNameIdentifierFormat = table.Column<string>(type: "TEXT", nullable: true),
                    EncryptionCertificate = table.Column<byte[]>(type: "BLOB", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelyingParties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RelyingPartyClaimMappings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    RelyingPartyId = table.Column<string>(type: "TEXT", nullable: true),
                    FromClaimType = table.Column<string>(type: "TEXT", nullable: false),
                    ToClaimType = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelyingPartyClaimMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelyingPartyClaimMappings_RelyingParties_RelyingPartyId",
                        column: x => x.RelyingPartyId,
                        principalTable: "RelyingParties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelyingPartyClaimMappings_RelyingPartyId",
                table: "RelyingPartyClaimMappings",
                column: "RelyingPartyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelyingPartyClaimMappings");

            migrationBuilder.DropTable(
                name: "RelyingParties");

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2020, 8, 13, 14, 4, 53, 252, DateTimeKind.Utc).AddTicks(6770));
        }
    }
}
