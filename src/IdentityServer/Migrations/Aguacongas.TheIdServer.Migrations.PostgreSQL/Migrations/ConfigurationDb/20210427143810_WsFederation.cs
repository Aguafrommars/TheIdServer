// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.PostgreSQL.Migrations.ConfigurationDb
{
    public partial class WsFederation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RelyingParties",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    TokenType = table.Column<string>(type: "text", nullable: false),
                    DigestAlgorithm = table.Column<string>(type: "text", nullable: false),
                    SignatureAlgorithm = table.Column<string>(type: "text", nullable: false),
                    SamlNameIdentifierFormat = table.Column<string>(type: "text", nullable: true),
                    EncryptionCertificate = table.Column<byte[]>(type: "bytea", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelyingParties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RelyingPartyClaimMappings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RelyingPartyId = table.Column<string>(type: "text", nullable: true),
                    FromClaimType = table.Column<string>(type: "text", nullable: false),
                    ToClaimType = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
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
        }
    }
}
