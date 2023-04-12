using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.PostgreSQL.Migrations.OperationalDb
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
                newName: "Data");

            migrationBuilder.AddColumn<DateTime>(
                name: "Expiration",
                table: "Saml2pArtifact",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Saml2pArtifact",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expiration",
                table: "Saml2pArtifact");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Saml2pArtifact");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Saml2pArtifact",
                newName: "Xml");
        }
    }
}
