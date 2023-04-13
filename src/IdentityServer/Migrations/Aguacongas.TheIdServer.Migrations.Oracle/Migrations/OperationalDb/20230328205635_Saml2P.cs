using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.Oracle.Migrations.OperationalDb
{
    public partial class Saml2P : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Saml2pArtifact",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar2(450)", nullable: false),
                    ClientId = table.Column<string>(type: "nclob", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar2(200)", maxLength: 200, nullable: true),
                    Xml = table.Column<string>(type: "nclob", nullable: true),
                    SessionId = table.Column<string>(type: "nclob", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Saml2pArtifact", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Saml2pArtifact");
        }
    }
}
