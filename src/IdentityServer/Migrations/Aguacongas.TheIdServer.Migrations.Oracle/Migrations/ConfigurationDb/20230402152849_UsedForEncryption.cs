using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.Oracle.Migrations.ConfigurationDb
{
    public partial class UsedForEncryption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignatureValidationCertificate",
                table: "Clients");
            
            migrationBuilder.AddColumn<bool>(
                name: "UsedForEncryption",
                table: "ClientSecrets",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedForEncryption",
                table: "ClientSecrets");

            migrationBuilder.AddColumn<byte[]>(
                name: "SignatureValidationCertificate",
                table: "Clients",
                nullable: true);
        }
    }
}
