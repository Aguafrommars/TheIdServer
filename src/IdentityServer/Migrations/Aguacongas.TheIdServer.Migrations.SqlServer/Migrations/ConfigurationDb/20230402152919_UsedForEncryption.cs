using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.SqlServer.Migrations.ConfigurationDb
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
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2023, 4, 2, 15, 29, 19, 112, DateTimeKind.Utc).AddTicks(3779));
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
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Cultures",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2023, 3, 29, 15, 26, 28, 338, DateTimeKind.Utc).AddTicks(8358));
        }
    }
}
