using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.PostgreSQL.Migrations.ConfigurationDb
{
    /// <inheritdoc />
    public partial class UsedForEncryption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignatureValidationCertificate",
                table: "Clients");

            migrationBuilder.AddColumn<bool>(
                name: "UsedForEncryption",
                table: "ClientSecrets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2023, 4, 2, 15, 29, 5, 388, DateTimeKind.Utc).AddTicks(2090));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedForEncryption",
                table: "ClientSecrets");

            migrationBuilder.AddColumn<byte[]>(
                name: "SignatureValidationCertificate",
                table: "Clients",
                type: "bytea",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2023, 3, 29, 15, 26, 6, 372, DateTimeKind.Utc).AddTicks(1778));
        }
    }
}
