using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.Oracle.Migrations.ConfigurationDb
{
    public partial class Saml2P : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Saml2PMetadata",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "SignatureValidationCertificate",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseAcsArtifact",
                table: "Clients",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Saml2PMetadata",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "SignatureValidationCertificate",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UseAcsArtifact",
                table: "Clients");
        }
    }
}
