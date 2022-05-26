using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.MySql.Migrations.ConfigurationDb
{
    public partial class NewClientOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientAllowedIdentityTokenSigningAlgorithms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: false),
                    Algorithm = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAllowedIdentityTokenSigningAlgorithms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientAllowedIdentityTokenSigningAlgorithms_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
                migrationBuilder.CreateIndex(
                name: "IX_ClientAllowedIdentityTokenSigningAlgorithms_ClientId_Algorit~",
                table: "ClientAllowedIdentityTokenSigningAlgorithms",
                columns: new[] { "ClientId", "Algorithm" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientAllowedIdentityTokenSigningAlgorithms");
        }
    }
}
