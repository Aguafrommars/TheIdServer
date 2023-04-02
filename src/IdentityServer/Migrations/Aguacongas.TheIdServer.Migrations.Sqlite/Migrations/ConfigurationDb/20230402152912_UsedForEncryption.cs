using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.Sqlite.Migrations.ConfigurationDb
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
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2023, 4, 2, 15, 29, 12, 245, DateTimeKind.Utc).AddTicks(3116));
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
                type: "BLOB",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2023, 3, 29, 15, 26, 16, 726, DateTimeKind.Utc).AddTicks(314));
        }
    }
}
