using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.PostgreSQL.Migrations.ConfigurationDb
{
    /// <inheritdoc />
    public partial class Saml2P : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Saml2PMetadata",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "SignatureValidationCertificate",
                table: "Clients",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseAcsArtifact",
                table: "Clients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2023, 3, 29, 15, 26, 6, 372, DateTimeKind.Utc).AddTicks(1778));
        }

        /// <inheritdoc />
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

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2022, 6, 29, 14, 23, 24, 594, DateTimeKind.Utc).AddTicks(7228));
        }
    }
}
