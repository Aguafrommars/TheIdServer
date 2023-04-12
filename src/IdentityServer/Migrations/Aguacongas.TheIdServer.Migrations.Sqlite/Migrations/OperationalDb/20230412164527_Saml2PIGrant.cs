using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.Sqlite.Migrations.OperationalDb
{
    /// <inheritdoc />
    public partial class Saml2PIGrant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Xml",
                table: "Saml2pArtifact",
                newName: "ModifiedAt");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "Saml2pArtifact",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Expiration",
                table: "Saml2pArtifact",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "Saml2pArtifact");

            migrationBuilder.DropColumn(
                name: "Expiration",
                table: "Saml2pArtifact");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "Saml2pArtifact",
                newName: "Xml");
        }
    }
}
