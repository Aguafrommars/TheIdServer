using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.MySql.Migrations.ConfigurationDb
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
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2023, 4, 2, 15, 28, 49, 560, DateTimeKind.Utc).AddTicks(1009));
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
                type: "longblob",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2023, 3, 29, 15, 25, 36, 811, DateTimeKind.Utc).AddTicks(7720));
        }
    }
}
