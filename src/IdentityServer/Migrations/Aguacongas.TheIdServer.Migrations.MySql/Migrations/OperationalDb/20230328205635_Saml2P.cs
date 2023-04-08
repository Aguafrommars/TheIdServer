using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.MySql.Migrations.OperationalDb
{
    /// <inheritdoc />
    public partial class Saml2P : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Saml2pArtifact",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    ClientId = table.Column<string>(type: "longtext", nullable: false),
                    UserId = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Xml = table.Column<string>(type: "longtext", nullable: true),
                    SessionId = table.Column<string>(type: "longtext", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Saml2pArtifact", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Saml2pArtifact");
        }
    }
}
