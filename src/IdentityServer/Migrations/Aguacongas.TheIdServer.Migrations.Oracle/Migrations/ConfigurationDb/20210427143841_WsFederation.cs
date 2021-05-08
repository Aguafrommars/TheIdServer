// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aguacongas.TheIdServer.Oracle.Migrations.ConfigurationDb
{
    public partial class WsFederation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RelyingParties",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    TokenType = table.Column<string>(nullable: false),
                    DigestAlgorithm = table.Column<string>(nullable: false),
                    SignatureAlgorithm = table.Column<string>(nullable: false),
                    SamlNameIdentifierFormat = table.Column<string>(nullable: true),
                    EncryptionCertificate = table.Column<byte[]>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelyingParties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RelyingPartyClaimMappings",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RelyingPartyId = table.Column<string>(nullable: true),
                    FromClaimType = table.Column<string>(nullable: false),
                    ToClaimType = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
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
