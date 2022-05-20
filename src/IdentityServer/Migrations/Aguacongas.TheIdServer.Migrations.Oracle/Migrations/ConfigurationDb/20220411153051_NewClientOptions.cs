using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aguacongas.TheIdServer.Oracle.Migrations.ConfigurationDb
{
    public partial class NewClientOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CoordinateLifetimeWithUserSession",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireRequestObject",
                table: "Clients",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ClientAllowedIdentityTokenSigningAlgorithm",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: false),
                    Algorithm = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAllowedIdentityTokenSigningAlgorithm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientAllowedIdentityTokenSigningAlgorithm_Clients_ClientId",
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
                name: "ClientAllowedIdentityTokenSigningAlgorithm");

            migrationBuilder.DropColumn(
                name: "CoordinateLifetimeWithUserSession",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "RequireRequestObject",
                table: "Clients");
        }
    }
}
